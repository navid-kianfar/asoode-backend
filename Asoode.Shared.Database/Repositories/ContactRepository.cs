using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class ContactRepository : IContactRepository
{
    private readonly ILoggerService _loggerService;

    public ContactRepository(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }
    
    public async Task<OperationResult<ContactReplyDto[]>> Replies(Guid userId, Guid id)
    {
        try
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactRepository.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<GridResult<ContactListDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model)
    {
        try
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactRepository.Contact", e);
            return OperationResult<bool>.Failed();
        }
    }
}