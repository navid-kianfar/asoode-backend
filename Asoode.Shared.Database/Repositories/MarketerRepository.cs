using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Marketer;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class MarketerRepository : IMarketerRepository
{
    private readonly ILoggerService _loggerService;

    public MarketerRepository(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }
    public async Task<OperationResult<GridResult<MarketerDto>>> List(Guid userId, GridFilter model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "MarketerRepository.Create", e);
            return OperationResult<GridResult<MarketerDto>>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Create(Guid userId, MarketerEditableDto model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "MarketerRepository.Create", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Edit(Guid userId, Guid id, MarketerEditableDto model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "MarketerRepository.Edit", e);
            return OperationResult<bool>.Failed();
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
            await _loggerService.Error(e.Message, "MarketerRepository.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Toggle(Guid userId, Guid id)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "MarketerRepository.Toggle", e);
            return OperationResult<bool>.Failed();
        }
    }
}