using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Reports;


[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Reports")]
public class ReportController : BaseController
{
    private readonly IReportService _reportBiz;
    private readonly IUserIdentityService _identity;

    public ReportController(IReportService reportBiz, IUserIdentityService identity)
    {
        _reportBiz = reportBiz;
        _identity = identity;
    }

    [HttpPost]
    [Route("reports/dashboard")]
    public async Task<IActionResult> Dashboard([FromBody] DashboardDurationDto model)
    {
        var op = await _reportBiz.Dashboard(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost]
    [Route("reports/activities/{id:guid?}")]
    public async Task<IActionResult> Activities(Guid? id)
    {
        var op = await _reportBiz.Activities(_identity.User!.UserId, id);
        return Json(op);
    }
}