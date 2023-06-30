using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Membership;


[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Plans")]
public class PlanController : BaseController
{
    private readonly IPlanService _planBiz;
    private readonly IUserIdentityService _identity;

    public PlanController(IPlanService planBiz, IUserIdentityService identity)
    {
        _planBiz = planBiz;
        _identity = identity;
    }

    
    [HttpPost("plans/fetch")]
    
    public async Task<IActionResult> Fetch()
    {
        var op = await _planBiz.Fetch(_identity.User!.UserId);
        return Json(op);
    }
}