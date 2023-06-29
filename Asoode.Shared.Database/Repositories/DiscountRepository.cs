using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Discount;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class DiscountRepository : IDiscountRepository
{
    private readonly ILoggerService _loggerService;

    public DiscountRepository(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }
    
    public async Task<OperationResult<bool>> Edit(Guid userId, Guid id, DiscountEditableDto model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "DiscountRepository.Edit", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Create(Guid userId, DiscountEditableDto model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "DiscountRepository.Create", e);
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
            await _loggerService.Error(e.Message, "DiscountRepository.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<GridResult<DiscountDto>>> List(Guid userId, GridFilter model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "DiscountRepository.List", e);
            return OperationResult<GridResult<DiscountDto>>.Failed();
        }
    }
}