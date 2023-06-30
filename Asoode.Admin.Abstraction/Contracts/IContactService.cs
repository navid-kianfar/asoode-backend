using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Types;
using ContactDto = Asoode.Shared.Abstraction.Dtos.Contact.ContactDto;
using ContactListDto = Asoode.Shared.Abstraction.Dtos.Contact.ContactListDto;

namespace Asoode.Admin.Abstraction.Contracts;

public interface IContactService
{
    Task<OperationResult<bool>> Contact(ContactDto model);
    Task<OperationResult<Shared.Abstraction.Dtos.General.GridResult<ContactListDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model);
    Task<OperationResult<bool>> Delete(Guid userId, Guid id);
    Task<OperationResult<bool>> Reply(Guid userId, Guid id, ContactReplyDto model);
    Task<OperationResult<ContactReplyDto[]>> Replies(Guid userId, Guid id);
}