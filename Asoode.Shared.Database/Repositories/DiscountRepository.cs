using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Discount;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class DiscountRepository : IDiscountRepository
{
    public Task<OperationResult<bool>> Edit(Guid userId, Guid id, DiscountEditableDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Create(Guid userId, DiscountEditableDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<GridResult<DiscountDto>>> List(Guid userId, GridFilter model)
    {
        throw new NotImplementedException();
    }
}