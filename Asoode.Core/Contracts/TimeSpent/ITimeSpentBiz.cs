using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Reports;

namespace Asoode.Core.Contracts.TimeSpent;

public interface ITimeSpentBiz
{
    Task<OperationResult<TimeSpentViewModel[]>> TimeSpents(Guid userId, DurationViewModel model);
    Task<OperationResult<TimeSpentViewModel[]>> GroupTimeSpents(Guid userId, Guid groupId, DurationViewModel model);
}