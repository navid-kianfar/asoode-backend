using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Application.Business.Implementation;

internal class PlanService : IPlanService
{
    private readonly IPlanRepository _repository;

    public PlanService(IPlanRepository repository)
    {
        _repository = repository;
    }

    public Task<OperationResult<PlansFetchDto>> Fetch(Guid userId)
        => _repository.Fetch(userId);
}