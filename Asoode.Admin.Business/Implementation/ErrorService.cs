using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Admin.Business.Implementation;

internal class ErrorService : IErrorService
{
    private readonly IErrorRepository _errorRepository;

    public ErrorService(IErrorRepository errorRepository)
    {
        _errorRepository = errorRepository;
    }

    public Task<OperationResult<GridResult<ErrorDto>>> List(Guid userId, GridFilter model)
        => _errorRepository.List(userId, model);

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
        => _errorRepository.Delete(userId, id);
}