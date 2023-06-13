using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.TimeSpent;
using Asoode.Core.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.TimeSpent;

[JwtAuthorize]
[Route("v2/times")]
[ApiExplorerSettings(GroupName = "Time Spent")]
public class TimeSpentController : BaseController
{
    private readonly ITimeSpentBiz _timeSpentBiz;

    public TimeSpentController(ITimeSpentBiz timeSpentBiz)
    {
        _timeSpentBiz = timeSpentBiz;
    }

    [HttpPost("mine")]
    [ValidateModel]
    public async Task<IActionResult> TimeSpents([FromBody] DurationViewModel model)
    {
        var op = await _timeSpentBiz.TimeSpents(Identity.UserId, model);
        return Json(op);
    }

    [HttpPost("group/{id:guid}")]
    [ValidateModel]
    public async Task<IActionResult> TimeSpent(Guid id, [FromBody] DurationViewModel model)
    {
        var op = await _timeSpentBiz
            .GroupTimeSpents(Identity.UserId, id, model);
        return Json(op);
    }
}