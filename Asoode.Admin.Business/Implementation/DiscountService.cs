using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class DiscountService : IDiscountService
{
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