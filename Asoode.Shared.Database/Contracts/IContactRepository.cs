using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface IContactRepository
{
    Task<OperationResult<bool>> Reply(Guid userId, Guid id, ContactReplyDto model);
    Task<OperationResult<ContactReplyDto[]>> Replies(Guid userId, Guid id);
    Task<OperationResult<bool>> Delete(Guid userId, Guid id);
    Task<OperationResult<GridResult<ContactListDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model);
    Task<OperationResult<bool>> Contact(ContactDto model);
    Task<OperationResult<ContactDto>> Get(Guid id);
}