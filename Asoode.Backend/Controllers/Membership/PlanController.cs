using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Membership;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Membership;

[JwtAuthorize]
[Route("v2/plans")]
[ApiExplorerSettings(GroupName = "Plans")]
public class PlanController : BaseController
{
    private readonly IPlanBiz _planBiz;

    public PlanController(IPlanBiz planBiz)
    {
        _planBiz = planBiz;
    }

    [JwtAuthorize]
    [HttpPost("fetch")]
    [ValidateModel]
    public async Task<IActionResult> Fetch()
    {
        var op = await _planBiz.Fetch(Identity.UserId);
        return Json(op);
    }
}