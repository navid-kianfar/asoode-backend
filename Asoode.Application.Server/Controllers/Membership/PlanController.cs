using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Membership;


[Route("v2/plans")]
[ApiExplorerSettings(GroupName = "Plans")]
public class PlanController : BaseController
{
    private readonly IPlanBiz _planBiz;

    public PlanController(IPlanBiz planBiz)
    {
        _planBiz = planBiz;
    }

    
    [HttpPost("fetch")]
    
    public async Task<IActionResult> Fetch()
    {
        var op = await _planBiz.Fetch(_identity.User!.UserId);
        return Json(op);
    }
}