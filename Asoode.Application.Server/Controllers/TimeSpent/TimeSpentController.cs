using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.TimeSpent;


[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Time Spent")]
public class TimeSpentController : BaseController
{
    private readonly ITimeSpentService _timeSpentBiz;
    private readonly IUserIdentityService _identity;

    public TimeSpentController(ITimeSpentService timeSpentBiz, IUserIdentityService identity)
    {
        _timeSpentBiz = timeSpentBiz;
        _identity = identity;
    }

    [HttpPost("times/mine")]
    
    public async Task<IActionResult> TimeSpents([FromBody] DurationDto model)
    {
        var op = await _timeSpentBiz.TimeSpents(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("times/group/{id:guid}")]
    
    public async Task<IActionResult> TimeSpent(Guid id, [FromBody] DurationDto model)
    {
        var op = await _timeSpentBiz
            .GroupTimeSpents(_identity.User!.UserId, id, model);
        return Json(op);
    }
}