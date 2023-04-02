using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.Reports
{
    [JwtAuthorize]
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
        public async Task<IActionResult> Dashboard([FromBody] DashboardDurationViewModel model)
        {
            var op = await _reportBiz.Dashboard(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost]
        [Route("activities/{id:guid?}")]
        public async Task<IActionResult> Activities(Guid? id)
        {
            var op = await _reportBiz.Activities(Identity.UserId, id);
            return Json(op);
        }
    }
}