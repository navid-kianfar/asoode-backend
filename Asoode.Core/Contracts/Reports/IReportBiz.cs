using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Reports;

namespace Asoode.Core.Contracts.Reports;

public interface IReportBiz
{
    Task<OperationResult<DashBoardViewModel>> Dashboard(Guid userId, DashboardDurationViewModel model);
    Task<OperationResult<WorkPackageTaskViewModel[]>> Activities(Guid identityUserId, Guid? id);
}