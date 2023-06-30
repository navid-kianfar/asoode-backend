using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface IReportService
{
    Task<OperationResult<DashBoardDto>> Dashboard(Guid userId, DashboardDurationDto model);
    Task<OperationResult<WorkPackageTaskDto[]>> Activities(Guid identityUserId, Guid? id);
}