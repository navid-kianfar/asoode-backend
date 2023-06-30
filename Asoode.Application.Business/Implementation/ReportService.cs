using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class ReportService : IReportService
{
    public Task<OperationResult<DashBoardDto>> Dashboard(Guid userId, DashboardDurationDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<WorkPackageTaskDto[]>> Activities(Guid identityUserId, Guid? id)
    {
        throw new NotImplementedException();
    }
}