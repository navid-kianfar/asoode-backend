using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class PlanService : IPlanService
{
    public Task<OperationResult<PlansFetchDto>> Fetch(Guid userId)
    {
        throw new NotImplementedException();
    }
}