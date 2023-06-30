using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface IPlanRepository
{
    Task<OperationResult<PlanDto[]>> List();
    Task<OperationResult<SelectableItem<Guid>[]>> All(Guid userId);
    Task<OperationResult<bool>> EditUserPlan(Guid userId, Guid id, UserPlanInfoDto model);
    Task<OperationResult<bool>> Edit(Guid userId, Guid id, PlanDto model);
    Task<OperationResult<bool>> Toggle(Guid userId, Guid id);
    Task<OperationResult<bool>> Create(Guid userId, PlanDto model);
    Task<OperationResult<GridResult<PlanDto>>> List(Guid userId, GridFilter model);
}