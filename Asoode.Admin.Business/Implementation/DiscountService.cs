using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ILoggerService _loggerService;

    public DiscountService(IDiscountRepository discountRepository, ILoggerService loggerService)
    {
        _discountRepository = discountRepository;
        _loggerService = loggerService;
    }
    public Task<OperationResult<GridResult<DiscountDto>>> List(Guid userId, GridFilter model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Create(Guid userId, DiscountEditableDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Edit(Guid userId, Guid id, DiscountEditableDto model)
    {
        throw new NotImplementedException();
    }
}