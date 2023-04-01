using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Collaboration;
using Asoode.Core.ViewModels.Collaboration;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Collaboration
{
    [JwtAuthorize]
    [Route("v2/groups")]
    [ApiExplorerSettings(GroupName = "Group")]
    public class GroupController : BaseController
    {
        private readonly IGroupBiz _groupBiz;

        public GroupController(IGroupBiz groupBiz)
        {
            _groupBiz = groupBiz;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var op = await _groupBiz.List(Identity.UserId);
            return Json(op);
        }
        
        [HttpPost("archived")]
        [ValidateModel]
        public async Task<IActionResult> Archived()
        {
            var op = await _groupBiz.Archived(Identity.UserId);
            return Json(op);
        }

        [HttpPost("create")]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] GroupCreateViewModel model)
        {
            var op = await _groupBiz.Create(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/edit")]
        [ValidateModel]
        public async Task<IActionResult> Edit(Guid id, [FromBody] GroupViewModel model)
        {
            var op = await _groupBiz.Edit(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/upgrade")]
        public async Task<IActionResult> Upgrade(Guid id)
        {
            var op = await _groupBiz.Upgrade(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/connect")]
        [ValidateModel]
        public async Task<IActionResult> Connect(Guid id, [FromBody]IdViewModel model)
        {
            var op = await _groupBiz.Connect(Identity.UserId, id, model.Id);
            return Json(op);
        }

        [HttpPost("{id:guid}/non-attached")]
        public async Task<IActionResult> NonAttached(Guid id)
        {
            var op = await _groupBiz.NonAttached(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("shifts/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Shifts(Guid id, [FromBody]GridFilter filter)
        {
            var op = await _groupBiz.Shifts(Identity.UserId, id, filter);
            return Json(op);
        }

        [HttpPost("shifts/{id:guid}/create")]
        [ValidateModel]
        public async Task<IActionResult> CreateShift(Guid id, [FromBody]EditShiftViewModel model)
        {
            var op = await _groupBiz.CreateShift(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("shifts/{id:guid}/edit")]
        [ValidateModel]
        public async Task<IActionResult> EditShift(Guid id, [FromBody]EditShiftViewModel model)
        {
            var op = await _groupBiz.EditShift(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("shifts/{id:guid}/remove")]
        [ValidateModel]
        public async Task<IActionResult> DeleteShift(Guid id)
        {
            var op = await _groupBiz.DeleteShift(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("time-offs/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> TimeOffs(Guid id, [FromBody]GridFilter filter)
        {
            var op = await _groupBiz.TimeOffs(Identity.UserId, id, filter);
            return Json(op);
        }

        [HttpPost("time-offs/{id:guid}/request")]
        [ValidateModel]
        public async Task<IActionResult> RequestTimeOff(Guid id, [FromBody]RequestTimeOffViewModel model)
        {
            var op = await _groupBiz.RequestTimeOff(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("time-offs/{id:guid}/history")]
        [ValidateModel]
        public async Task<IActionResult> TimeOffHistory(Guid id, [FromBody]GridFilterWithParams<IdViewModel> filter)
        {
            var op = await _groupBiz.TimeOffHistory(Identity.UserId, id, filter);
            return Json(op);
        }

        [HttpPost("time-offs/{id:guid}/detail")]
        public async Task<IActionResult> TimeOffDetail(Guid id)
        {
            var op = await _groupBiz.TimeOffDetail(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("time-offs/{id:guid}/approve")]
        public async Task<IActionResult> TimeOffApprove(Guid id)
        {
            var op = await _groupBiz.TimeOffResponse(Identity.UserId, id, true);
            return Json(op);
        }

        [HttpPost("time-offs/{id:guid}/delete")]
        public async Task<IActionResult> RemoveTimeOff(Guid id)
        {
            var op = await _groupBiz.RemoveTimeOff(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("time-offs/{id:guid}/decline")]
        public async Task<IActionResult> TimeOffDecline(Guid id)
        {
            var op = await _groupBiz.TimeOffResponse(Identity.UserId, id, false);
            return Json(op);
        }

        [HttpPost("entry-logs/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> EntryLogs(Guid id, [FromBody]GridFilter filter)
        {
            var op = await _groupBiz.EntryLogs(Identity.UserId, id, filter);
            return Json(op);
        }

        [HttpPost("toggle-entry/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> ToggleEntry(Guid id)
        {
            var op = await _groupBiz.ToggleEntry(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("remove-entry/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> RemoveEntry(Guid id)
        {
            var op = await _groupBiz.RemoveEntry(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("edit-entry/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> EditEntry(Guid id, [FromBody]OptionalDurationViewModel model)
        {
            var op = await _groupBiz.EditEntry(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("manual-entry/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> ManualEntry(Guid id, [FromBody]ManualEntryViewModel model)
        {
            var op = await _groupBiz.ManualEntry(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("remove-access/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> RemoveAccess(Guid id)
        {
            var op = await _groupBiz.RemoveAccess(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("change-access/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> ChangeAccess(Guid id, [FromBody] ChangeAccessViewModel permission)
        {
            var op = await _groupBiz.ChangeAccess(Identity.UserId, id, permission);
            return Json(op);
        }

        [HttpPost("remove-pending-access/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> RemovePendingAccess(Guid id)
        {
            var op = await _groupBiz.RemovePendingAccess(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("change-pending-access/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> ChangePendingAccess(Guid id, [FromBody] ChangeAccessViewModel permission)
        {
            var op = await _groupBiz.ChangePendingAccess(Identity.UserId, id, permission);
            return Json(op);
        }

        [HttpPost("export/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Export(Guid id)
        {
            var op = await _groupBiz.Export(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/remove")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var op = await _groupBiz.Remove(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/archive")]
        public async Task<IActionResult> Archive(Guid id)
        {
            var op = await _groupBiz.Archive(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("{id:guid}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            var op = await _groupBiz.Restore(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("{id:guid}/fetch")]
        public async Task<IActionResult> Fetch(Guid id)
        {
            var op = await _groupBiz.Fetch(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("{id:guid}/report")]
        public async Task<IActionResult> Report(Guid id, [FromBody]DurationViewModel model)
        {
            var op = await _groupBiz.Report(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/add-access")]
        [ValidateModel]
        public async Task<IActionResult> AddAccess(Guid id, [FromBody] AccessViewModel permission)
        {
            var op = await _groupBiz.AddAccess(Identity.UserId, id, permission);
            return Json(op);
        }
    }
}