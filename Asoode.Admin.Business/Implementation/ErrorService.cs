using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class ErrorService : IErrorService
{
    private readonly IErrorRepository _errorRepository;
    private readonly ILoggerService _loggerService;

    public ErrorService(IErrorRepository errorRepository, ILoggerService loggerService)
    {
        _errorRepository = errorRepository;
        _loggerService = loggerService;
    }
    public Task<OperationResult<GridResult<ErrorDto>>> List(Guid userId, GridFilter model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}