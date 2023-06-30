using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface IPlanService
{
    Task<OperationResult<PlansFetchDto>> Fetch(Guid userId);
}