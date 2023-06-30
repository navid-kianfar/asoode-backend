using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Discount;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Abstraction.Contracts;

public interface IDiscountService
{
    Task<OperationResult<GridResult<DiscountDto>>> List(Guid userId, GridFilter model);
    Task<OperationResult<bool>> Delete(Guid userId, Guid id);
    Task<OperationResult<bool>> Create(Guid userId, DiscountEditableDto model);
    Task<OperationResult<bool>> Edit(Guid userId, Guid id, DiscountEditableDto model);
}