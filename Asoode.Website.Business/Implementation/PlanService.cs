using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;
using Asoode.Website.Abstraction.Contracts;

namespace Asoode.Website.Business.Implementation;

internal class PlanService : IPlanService
{
    private readonly IPlanRepository _repository;

    public PlanService(IPlanRepository repository)
    {
        _repository = repository;
    }

    public Task<OperationResult<PlanDto[]>> List()
        => _repository.List();
}