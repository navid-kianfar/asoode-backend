using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;
using ContactDto = Asoode.Shared.Abstraction.Dtos.Contact.ContactDto;
using ContactListDto = Asoode.Shared.Abstraction.Dtos.Contact.ContactListDto;

namespace Asoode.Admin.Business.Implementation;

internal class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly ILoggerService _loggerService;
    private readonly IPostmanService _postmanService;

    public ContactService(
        IContactRepository contactRepository, 
        ILoggerService loggerService,
        IPostmanService postmanService
        )
    {
        _contactRepository = contactRepository;
        _loggerService = loggerService;
        _postmanService = postmanService;
    }
    public Task<OperationResult<bool>> Contact(ContactDto model)
        => _contactRepository.Contact(model);

    public Task<OperationResult<Shared.Abstraction.Dtos.General.GridResult<ContactListDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model)
        => _contactRepository.List(userId, model);

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
        => _contactRepository.Delete(userId, id);

    public async Task<OperationResult<bool>> Reply(Guid userId, Guid id, ContactReplyDto model)
    {
        try
        {
            var contactOp = await _contactRepository.Get(id);
            if (contactOp.Status != OperationResultStatus.Success)
                return OperationResult<bool>.Failed();
            
            // TODO: fix this
            // var sendOp = await _postmanService.ReplyContact(contactOp.Data!, model.Message);
            // if (sendOp.Status != OperationResultStatus.Success)
            //     return OperationResult<bool>.Failed();

            return await _contactRepository.Reply(userId, id, model);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ContactService.Reply", e);
            return OperationResult<bool>.Failed();
        }
    }

    public Task<OperationResult<ContactReplyDto[]>> Replies(Guid userId, Guid id)
        => _contactRepository.Replies(userId, id);
}