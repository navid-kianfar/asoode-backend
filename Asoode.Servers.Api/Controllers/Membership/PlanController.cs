using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.Membership
{
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
}