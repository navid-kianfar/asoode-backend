using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Admin;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Membership;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Membership.Plan;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Admin
{
    [JwtAuthorize(UserType.Admin)]
    [Route("v2/admin/contact")]
    public class AdminContactController : BaseController
    {
        private readonly IContactBiz _contactBiz;

        public AdminContactController(IContactBiz contactBiz)
        {
            _contactBiz = contactBiz;
        }
        
        [ValidateModel]
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] GridFilterWithParams<GridQuery> model)
        {
            var op = await _contactBiz.List(Identity.UserId, model);
            return Json(op);
        }
        
        [HttpPost("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var op = await _contactBiz.Delete(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("replies/{id:guid}")]
        public async Task<IActionResult> Replies(Guid id)
        {
            var op = await _contactBiz.Replies(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("reply/{id:guid}")]
        public async Task<IActionResult> Reply(Guid id, [FromBody]ContactReplyViewModel model)
        {
            var op = await _contactBiz.Reply(Identity.UserId, id, model);
            return Json(op);
        }
    }
}