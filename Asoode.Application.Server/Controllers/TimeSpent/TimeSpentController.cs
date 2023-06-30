using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.TimeSpent;


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
    
    public async Task<IActionResult> TimeSpents([FromBody] DurationDto model)
    {
        var op = await _timeSpentBiz.TimeSpents(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("group/{id:guid}")]
    
    public async Task<IActionResult> TimeSpent(Guid id, [FromBody] DurationDto model)
    {
        var op = await _timeSpentBiz
            .GroupTimeSpents(_identity.User!.UserId, id, model);
        return Json(op);
    }
}