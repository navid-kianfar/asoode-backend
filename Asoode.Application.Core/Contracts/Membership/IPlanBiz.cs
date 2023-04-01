using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Membership.Plan;

namespace Asoode.Application.Core.Contracts.Membership;

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