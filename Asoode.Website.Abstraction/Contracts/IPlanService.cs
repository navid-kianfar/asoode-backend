using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Website.Abstraction.Contracts;

public interface IPlanService
{
    Task<OperationResult<PlanDto[]>> List();
}