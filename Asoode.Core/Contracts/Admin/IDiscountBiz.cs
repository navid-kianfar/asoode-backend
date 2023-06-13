using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.Contracts.Admin;

public interface IDiscountBiz
{
    Task<OperationResult<GridResult<DiscountViewModel>>> AdminList(Guid userId, GridFilter model);
    Task<OperationResult<bool>> AdminDelete(Guid userId, Guid id);
    Task<OperationResult<bool>> AdminCreate(Guid userId, DiscountEditableViewModel model);
    Task<OperationResult<bool>> AdminEdit(Guid userId, Guid id, DiscountEditableViewModel model);
}