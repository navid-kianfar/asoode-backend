using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class TimeSpentService : ITimeSpentService
{
    public Task<OperationResult<TimeSpentDto[]>> TimeSpents(Guid userId, DurationDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<TimeSpentDto[]>> GroupTimeSpents(Guid userId, Guid groupId, DurationDto model)
    {
        throw new NotImplementedException();
    }
}