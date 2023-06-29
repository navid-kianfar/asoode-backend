using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class PlanService : IPlanService
{
    public Task<OperationResult<GridResult<PlanDto>>> List(Guid userId, GridFilter model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Create(Guid userId, PlanDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Toggle(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Edit(Guid userId, Guid id, PlanDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditUserPlan(Guid userId, Guid id, UserPlanInfoDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<SelectableItem<Guid>[]>> All(Guid userId)
    {
        throw new NotImplementedException();
    }
}