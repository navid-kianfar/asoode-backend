using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Error;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class ErrorRepository : IErrorRepository
{
    private readonly ILoggerService _loggerService;

    public ErrorRepository(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }

    public async Task<OperationResult<GridResult<ErrorDto>>> List(Guid userId, GridFilter model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "ErrorRepository.List", e);
            return OperationResult<GridResult<ErrorDto>>.Failed();
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
            await _loggerService.Error(e.Message, "ErrorRepository.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }
}