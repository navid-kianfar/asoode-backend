using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Membership.Plan;

namespace Asoode.Core.Contracts.Membership;

public interface IPlanBiz
{
    Task<OperationResult<PlansFetchViewModel>> Fetch(Guid userId);
    Task<OperationResult<GridResult<PlanViewModel>>> AdminPlansList(Guid userId, GridFilter model);
    Task<OperationResult<bool>> AdminCreate(Guid userId, PlanViewModel model);
    Task<OperationResult<bool>> AdminToggle(Guid userId, Guid id);
    Task<OperationResult<bool>> AdminEdit(Guid userId, Guid id, PlanViewModel model);
    Task<OperationResult<bool>> AdminEditUser(Guid userId, Guid id, UserPlanInfoViewModel model);
    Task<OperationResult<SelectableItem<Guid>[]>> AdminPlansAll(Guid userId);
}