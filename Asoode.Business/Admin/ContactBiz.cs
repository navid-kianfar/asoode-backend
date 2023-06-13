using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Business.Extensions;
using Asoode.Core.Contracts.Admin;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Z.EntityFramework.Plus;

namespace Asoode.Business.Admin;

internal class ContactBiz : IContactBiz
{
    private readonly IServiceProvider _serviceProvider;

    public ContactBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<OperationResult<bool>> Contact(ContactViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                await unit.Contacts.AddAsync(new Contact
                {
                    Email = model.Email,
                    Message = model.Message,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                });
                await unit.SaveChangesAsync();
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<GridResult<ContactListViewModel>>> List(Guid userId,
        GridFilterWithParams<GridQuery> model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var query = unit.Contacts
                    .Where(i => !i.DeletedAt.HasValue)
                    .OrderBy(i => i.CreatedAt);

                return await DbHelper.GetPaginatedData(query, tuple =>
                {
                    var (items, startIndex) = tuple;
                    return items.Select((i, index) => i.ToViewModel(startIndex + index + 1)).ToArray();
                }, model.Page, model.PageSize);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<GridResult<ContactListViewModel>>.Failed();
        }
    }

    public async Task<OperationResult<ContactReplyViewModel[]>> Replies(Guid userId, Guid id)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var contact = await unit.Contacts.SingleOrDefaultAsync(i => i.Id == id);
                if (contact == null) return OperationResult<ContactReplyViewModel[]>.NotFound();

                contact.Seen = true;

                var replies = await unit.ContactReplies.Where(i => i.ContactId == id)
                    .AsNoTracking()
                    .ToArrayAsync();

                var result = replies.Select(r => r.ToViewModel()).ToArray();

                return OperationResult<ContactReplyViewModel[]>.Success(result);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<ContactReplyViewModel[]>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Reply(Guid userId, Guid id, ContactReplyViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var contact = await unit.Contacts.SingleOrDefaultAsync(i => i.Id == id && !i.DeletedAt.HasValue);
                if (contact == null) return OperationResult<bool>.NotFound();

                var op = await _serviceProvider.GetService<IPostmanBiz>().Reply(contact.Email, model.Message);
                if (op.Status != OperationResultStatus.Success)
                    return OperationResult<bool>.Failed();

                await unit.ContactReplies.AddAsync(new ContactReply
                {
                    Message = model.Message,
                    ContactId = id,
                    UserId = userId
                });
                await unit.SaveChangesAsync();
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                await unit.Contacts.Where(i => i.Id == id).UpdateAsync(i => new Contact
                {
                    DeletedAt = DateTime.Now
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }
}