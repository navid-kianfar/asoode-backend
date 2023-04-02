using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.TimeSpent
{
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
        public async Task<IActionResult> TimeSpents([FromBody]DurationViewModel model)
        {
            var op = await _timeSpentBiz.TimeSpents(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("group/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> TimeSpent(Guid id, [FromBody]DurationViewModel model)
        {
            var op = await _timeSpentBiz
                .GroupTimeSpents(Identity.UserId, id, model);
            return Json(op);
        }
    }
}