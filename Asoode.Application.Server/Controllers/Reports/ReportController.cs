using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Reports;


[Route("v2/reports")]
[ApiExplorerSettings(GroupName = "Reports")]
public class ReportController : BaseController
{
    private readonly IReportBiz _reportBiz;

    public ReportController(IReportBiz reportBiz)
    {
        _reportBiz = reportBiz;
    }

    [HttpPost]
    [Route("dashboard")]
    public async Task<IActionResult> Dashboard([FromBody] DashboardDurationDto model)
    {
        var op = await _reportBiz.Dashboard(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost]
    [Route("activities/{id:guid?}")]
    public async Task<IActionResult> Activities(Guid? id)
    {
        var op = await _reportBiz.Activities(_identity.User!.UserId, id);
        return Json(op);
    }
}