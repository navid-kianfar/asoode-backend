using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Extensions;
using Asoode.Shared.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class ContactRepository : IContactRepository
{
    private readonly WebsiteContext _context;
    private readonly ILoggerService _loggerService;

    public ContactRepository(ILoggerService loggerService, WebsiteContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }

    public async Task<OperationResult<bool>> Reply(Guid userId, Guid id, ContactReplyDto model)
    {
        try
        {
            var contact = await _context.Contacts.SingleOrDefaultAsync(i => i.Id == id && !i.DeletedAt.HasValue);
            if (contact == null) return OperationResult<bool>.NotFound();

            await _context.ContactReplies.AddAsync(new ContactReply
            {
                Message = model.Message,
                ContactId = id,
                UserId = userId
            });
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactRepository.Reply", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<ContactReplyDto[]>> Replies(Guid userId, Guid id)
    {
        try
        {
            var contact = await _context.Contacts.SingleOrDefaultAsync(i => i.Id == id);
            if (contact == null) return OperationResult<ContactReplyDto[]>.NotFound();

            contact.Seen = true;

            var replies = await _context.ContactReplies.Where(i => i.ContactId == id)
                .AsNoTracking()
                .ToArrayAsync();

            var result = replies.Select(r => r.ToDto()).ToArray();

            return OperationResult<ContactReplyDto[]>.Success(result);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactRepository.Replies", e);
            return OperationResult<ContactReplyDto[]>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        try
        {
            await _context.Contacts
                .Where(i => i.Id == id)
                .ExecuteUpdateAsync(i => i.SetProperty(p => p.DeletedAt, DateTime.UtcNow));
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactRepository.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<GridResult<ContactListDto>>> List(Guid userId,
        GridFilterWithParams<GridQuery> model)
    {
        try
        {
            var query = _context.Contacts
                .Where(i => !i.DeletedAt.HasValue)
                .OrderBy(i => i.CreatedAt);

            return await DbHelper.GetPaginatedData(query, tuple =>
            {
                var (items, startIndex) = tuple;
                return items.Select((i, index) => i.ToDto(startIndex + index + 1)).ToArray();
            }, model.Page, model.PageSize);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactRepository.List", e);
            return OperationResult<GridResult<ContactListDto>>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Contact(ContactDto model)
    {
        try
        {
            await _context.Contacts.AddAsync(new Contact
            {
                Email = model.Email,
                Message = model.Message,
                FirstName = model.FirstName,
                LastName = model.LastName
            });
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactRepository.Contact", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<ContactDto>> Get(Guid id)
    {
        try
        {
            var contact = await _context.Contacts.Where(c => c.Id == id)
                .AsNoTracking()
                .FirstAsync();

            return OperationResult<ContactDto>.Success(contact.ToDto(0));
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactRepository.Get", e);
            return OperationResult<ContactDto>.Failed();
        }
    }
}