using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface IPlanRepository
{
    Task<OperationResult<PlanDto[]>> List();
}