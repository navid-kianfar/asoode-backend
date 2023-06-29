using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Admin.Business.Implementation;

internal class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly ILoggerService _loggerService;

    public ContactService(IContactRepository contactRepository, ILoggerService loggerService)
    {
        _contactRepository = contactRepository;
        _loggerService = loggerService;
    }
    public Task<OperationResult<bool>> Contact(ContactDto model)
        => _contactRepository.Contact(model);

    public Task<OperationResult<GridResult<ContactListDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model)
        => _contactRepository.List(userId, model);

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
        => _contactRepository.Delete(userId, id);

    public Task<OperationResult<bool>> Reply(Guid userId, Guid id, ContactReplyDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ContactReplyDto[]>> Replies(Guid userId, Guid id)
        => _contactRepository.Replies(userId, id);
}