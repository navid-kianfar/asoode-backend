using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Reports;

namespace Asoode.Application.Core.Contracts.Reports;

public interface IReportBiz
{
    Task<OperationResult<DashBoardViewModel>> Dashboard(Guid userId, DashboardDurationViewModel model);
    Task<OperationResult<WorkPackageTaskViewModel[]>> Activities(Guid identityUserId, Guid? id);
}