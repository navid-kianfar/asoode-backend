using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Admin.Business.Implementation;

internal class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;

    public PlanService(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public Task<OperationResult<GridResult<PlanDto>>> List(Guid userId, GridFilter model)
        => _planRepository.List(userId, model);

    public Task<OperationResult<bool>> Create(Guid userId, PlanDto model)
        => _planRepository.Create(userId, model);

    public Task<OperationResult<bool>> Toggle(Guid userId, Guid id)
        => _planRepository.Toggle(userId, id);

    public Task<OperationResult<bool>> Edit(Guid userId, Guid id, PlanDto model)
        => _planRepository.Edit(userId, id, model);

    public Task<OperationResult<bool>> EditUserPlan(Guid userId, Guid id, UserPlanInfoDto model)
        => _planRepository.EditUserPlan(userId, id, model);

    public Task<OperationResult<SelectableItem<Guid>[]>> All(Guid userId)
        => _planRepository.All(userId);
}