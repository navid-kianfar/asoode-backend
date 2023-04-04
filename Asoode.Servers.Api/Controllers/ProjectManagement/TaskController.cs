using Asoode.Application.Core.Contracts.ProjectManagement;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Reports;
using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Extensions;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.ProjectManagement
{
    [JwtAuthorize]
    [Route("v2/tasks")]
    [ApiExplorerSettings(GroupName = "Tasks")]
    public class TaskController : BaseController
    {
        private readonly ITaskBiz _taskBiz;

        public TaskController(ITaskBiz taskBiz)
        {
            _taskBiz = taskBiz;
        }

        [HttpPost("{id:guid}/create")]
        [ValidateModel]
        public async Task<IActionResult> Creat(Guid id, [FromBody] CreateTaskViewModel model)
        {
            var op = await _taskBiz.Create(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/attach")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> AddAttachment(Guid id)
        {
            var file = await Request.Form.Files?.FirstOrDefault()?.ToViewModel();
            var op = await _taskBiz.AddAttachment(Identity.UserId, id, file);
            return Json(op);
        }

        [HttpPost("{id:guid}/watch")]
        [ValidateModel]
        public async Task<IActionResult> Watch(Guid id)
        {
            var op = await _taskBiz.Watch(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/archive")]
        [ValidateModel]
        public async Task<IActionResult> Archive(Guid id)
        {
            var op = await _taskBiz.Archive(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("attachment/{id:guid}/rename")]
        [ValidateModel]
        public async Task<IActionResult> RenameAttachment(Guid id, [FromBody] TitleViewModel model)
        {
            var op = await _taskBiz.RenameAttachment(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("attachment/{id:guid}/remove")]
        [ValidateModel]
        public async Task<IActionResult> RemoveAttachment(Guid id)
        {
            var op = await _taskBiz.RemoveAttachment(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("attachment/{id:guid}/cover")]
        [ValidateModel]
        public async Task<IActionResult> CoverAttachment(Guid id)
        {
            var op = await _taskBiz.CoverAttachment(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/reposition")]
        [ValidateModel]
        public async Task<IActionResult> Reposition(Guid id, [FromBody] RepositionViewModel model)
        {
            var op = await _taskBiz.Reposition(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/move")]
        [ValidateModel]
        public async Task<IActionResult> Move(Guid id, [FromBody] MoveTaskViewModel model)
        {
            var op = await _taskBiz.Move(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/location")]
        [ValidateModel]
        public async Task<IActionResult> Location(Guid id, [FromBody] LocationViewModel model)
        {
            var op = await _taskBiz.Location(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/detail")]
        [ValidateModel]
        public async Task<IActionResult> Detail(Guid id)
        {
            var op = await _taskBiz.Detail(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/comment")]
        [ValidateModel]
        public async Task<IActionResult> Comment(Guid id, [FromBody] PostTaskCommentViewModel model)
        {
            var op = await _taskBiz.Comment(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/estimated")]
        [ValidateModel]
        public async Task<IActionResult> Estimated(Guid id, [FromBody] EstimatedTimeViewModel model)
        {
            var op = await _taskBiz.Estimated(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/spend-time")]
        [ValidateModel]
        public async Task<IActionResult> SpendTime(Guid id, [FromBody] DurationViewModel model)
        {
            var op = await _taskBiz.SpendTime(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/change-title")]
        [ValidateModel]
        public async Task<IActionResult> ChangeTitle(Guid id, [FromBody] TitleViewModel model)
        {
            var op = await _taskBiz.ChangeTitle(Identity.UserId, id, model);
            return Json(op);
        }
        
        [HttpPost("{id:guid}/vote")]
        [ValidateModel]
        public async Task<IActionResult> Vote(Guid id, [FromBody] VoteViewModel model)
        {
            var op = await _taskBiz.Vote(Identity.UserId, id, model);
            return Json(op);
        }
        [HttpPost("{id:guid}/vote/setting")]
        [ValidateModel]
        public async Task<IActionResult> VoteSetting(Guid id, [FromBody] VoteSettingViewModel model)
        {
            var op = await _taskBiz.VoteSetting(Identity.UserId, id, model);
            return Json(op);
        }
        [HttpPost("{id:guid}/vote/clear")]
        [ValidateModel]
        public async Task<IActionResult> VoteClear(Guid id)
        {
            var op = await _taskBiz.VoteClear(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("{id:guid}/set-date")]
        [ValidateModel]
        public async Task<IActionResult> SetDate(Guid id, [FromBody]SetDateViewModel model)
        {
            var op = await _taskBiz.SetDate(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/change-description")]
        [ValidateModel]
        public async Task<IActionResult> ChangeDescription(Guid id, [FromBody] TitleViewModel model)
        {
            var op = await _taskBiz.ChangeDescription(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/change-state")]
        [ValidateModel]
        public async Task<IActionResult> ChangeState(Guid id, [FromBody] StateViewModel model)
        {
            var op = await _taskBiz.ChangeState(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/member/add")]
        [ValidateModel]
        public async Task<IActionResult> AddMember(Guid id, [FromBody] TaskMemberViewModel model)
        {
            var op = await _taskBiz.AddMember(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{taskId:guid}/member/{recordId:guid}/remove")]
        public async Task<IActionResult> RemoveMember(Guid taskId, Guid recordId)
        {
            var op = await _taskBiz.RemoveMember(Identity.UserId, taskId, recordId);
            return Json(op);
        }

        [HttpPost("{taskId:guid}/label/add/{labelId:guid}")]
        public async Task<IActionResult> AddLabel(Guid taskId, Guid labelId)
        {
            var op = await _taskBiz.AddLabel(Identity.UserId, taskId, labelId);
            return Json(op);
        }

        [HttpPost("{taskId:guid}/logs")]
        public async Task<IActionResult> Logs(Guid taskId)
        {
            var op = await _taskBiz.Logs(Identity.UserId, taskId);
            return Json(op);
        }

        [HttpPost("{taskId:guid}/label/{labelId:guid}/remove")]
        public async Task<IActionResult> RemoveLabel(Guid taskId, Guid labelId)
        {
            var op = await _taskBiz.RemoveLabel(Identity.UserId, taskId, labelId);
            return Json(op);
        }
        
        [HttpPost("calendar")]
        public async Task<IActionResult> Calendar([FromBody]DurationViewModel model)
        {
            var op = await _taskBiz.Calendar(Identity.UserId, model);
            return Json(op);
        }
        
        [HttpPost("kartabl")]
        public async Task<IActionResult> Kartabl([FromBody]DurationViewModel model)
        {
            var op = await _taskBiz.Kartabl(Identity.UserId, model);
            return Json(op);
        }
    }
}