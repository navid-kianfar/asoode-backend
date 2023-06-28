using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class ContactService : IContactService
{
    public Task<OperationResult<bool>> Contact(ContactDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<GridResult<ContactListDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Reply(Guid userId, Guid id, ContactReplyDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ContactReplyDto[]>> Replies(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}