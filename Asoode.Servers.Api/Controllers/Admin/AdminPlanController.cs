using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Membership;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Membership.Plan;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Admin
{
    [JwtAuthorize(UserType.Admin)]
    [Route("v2/admin/plan")]
    public class AdminPlanController : BaseController
    {
        private readonly IPlanBiz _planBiz;

        public AdminPlanController(IPlanBiz planBiz)
        {
            _planBiz = planBiz;
        }
        
        [ValidateModel]
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] GridFilter model)
        {
            var op = await _planBiz.AdminPlansList(Identity.UserId, model);
            return Json(op);
        }
        
        [HttpPost("all")]
        public async Task<IActionResult> All()
        {
            var op = await _planBiz.AdminPlansAll(Identity.UserId);
            return Json(op);
        }
        
        [ValidateModel]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PlanViewModel model)
        {
            var op = await _planBiz.AdminCreate(Identity.UserId, model);
            return Json(op);
        }
        
        [HttpPost("toggle/{id:guid}")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var op = await _planBiz.AdminToggle(Identity.UserId, id);
            return Json(op);
        }
        
        [ValidateModel]
        [HttpPost("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] PlanViewModel model)
        {
            var op = await _planBiz.AdminEdit(Identity.UserId, id, model);
            return Json(op);
        }
        
        [ValidateModel]
        [HttpPost("user/{id:guid}")]
        public async Task<IActionResult> EditUser(Guid id, [FromBody] UserPlanInfoViewModel model)
        {
            var op = await _planBiz.AdminEditUser(Identity.UserId, id, model);
            return Json(op);
        }
    }
}