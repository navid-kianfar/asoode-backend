using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class ContactRepository : IContactRepository
{
    public Task<OperationResult<ContactReplyDto[]>> Replies(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<GridResult<ContactListDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Contact(ContactDto model)
    {
        throw new NotImplementedException();
    }
}