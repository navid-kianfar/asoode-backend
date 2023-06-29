using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Marketer;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Admin.Business.Implementation;

internal class MarketerService : IMarketerService
{
    private readonly IMarketerRepository _marketerRepository;

    public MarketerService(IMarketerRepository marketerRepository)
    {
        _marketerRepository = marketerRepository;
    }
    public Task<OperationResult<GridResult<MarketerDto>>> List(Guid userId, GridFilter model)
        => _marketerRepository.List(userId, model);

    public Task<OperationResult<bool>> Create(Guid userId, MarketerEditableDto model)
        => _marketerRepository.Create(userId, model);

    public Task<OperationResult<bool>> Edit(Guid userId, Guid id, MarketerEditableDto model)
        => _marketerRepository.Edit(userId, id, model);

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
        => _marketerRepository.Delete(userId, id);

    public Task<OperationResult<bool>> Toggle(Guid userId, Guid id)
        => _marketerRepository.Toggle(userId, id);
}