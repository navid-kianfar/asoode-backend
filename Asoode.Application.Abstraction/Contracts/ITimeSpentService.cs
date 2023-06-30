using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface ITimeSpentService
{
    Task<OperationResult<TimeSpentDto[]>> TimeSpents(Guid userId, DurationDto model);
    Task<OperationResult<TimeSpentDto[]>> GroupTimeSpents(Guid userId, Guid groupId, DurationDto model);
}