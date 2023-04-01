using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Reports;

namespace Asoode.Application.Core.Contracts.TimeSpent;

public interface ITimeSpentBiz
{
    Task<OperationResult<TimeSpentViewModel[]>> TimeSpents(Guid userId, DurationViewModel model);
    Task<OperationResult<TimeSpentViewModel[]>> GroupTimeSpents(Guid userId, Guid groupId, DurationViewModel model);
}