using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Collaboration;


[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Group")]
public class GroupController : BaseController
{
    private readonly IGroupService _groupBiz;
    private readonly IUserIdentityService _identity;

    public GroupController(IGroupService groupBiz, IUserIdentityService identity)
    {
        _groupBiz = groupBiz;
        _identity = identity;
    }

    [HttpPost("groups/list")]
    public async Task<IActionResult> List()
    {
        var op = await _groupBiz.List(_identity.User!.UserId);
        return Json(op);
    }

    [HttpPost("groups/archived")]
    public async Task<IActionResult> Archived()
    {
        var op = await _groupBiz.Archived(_identity.User!.UserId);
        return Json(op);
    }

    [HttpPost("groups/create")]
    public async Task<IActionResult> Create([FromBody] GroupCreateDto model)
    {
        var op = await _groupBiz.Create(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/edit")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] GroupDto model)
    {
        var op = await _groupBiz.Edit(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/upgrade")]
    public async Task<IActionResult> Upgrade(Guid id)
    {
        var op = await _groupBiz.Upgrade(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/connect")]
    public async Task<IActionResult> Connect(Guid id, [FromBody] IdDto model)
    {
        var op = await _groupBiz.Connect(_identity.User!.UserId, id, model.Id);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/non-attached")]
    public async Task<IActionResult> NonAttached(Guid id)
    {
        var op = await _groupBiz.NonAttached(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/shifts/{id:guid}")]
    public async Task<IActionResult> Shifts(Guid id, [FromBody] GridFilter filter)
    {
        var op = await _groupBiz.Shifts(_identity.User!.UserId, id, filter);
        return Json(op);
    }

    [HttpPost("groups/shifts/{id:guid}/create")]
    public async Task<IActionResult> CreateShift(Guid id, [FromBody] EditShiftDto model)
    {
        var op = await _groupBiz.CreateShift(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("groups/shifts/{id:guid}/edit")]
    public async Task<IActionResult> EditShift(Guid id, [FromBody] EditShiftDto model)
    {
        var op = await _groupBiz.EditShift(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("groups/shifts/{id:guid}/remove")]
    public async Task<IActionResult> DeleteShift(Guid id)
    {
        var op = await _groupBiz.DeleteShift(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/time-offs/{id:guid}")]
    public async Task<IActionResult> TimeOffs(Guid id, [FromBody] GridFilter filter)
    {
        var op = await _groupBiz.TimeOffs(_identity.User!.UserId, id, filter);
        return Json(op);
    }

    [HttpPost("groups/time-offs/{id:guid}/request")]
    public async Task<IActionResult> RequestTimeOff(Guid id, [FromBody] RequestTimeOffDto model)
    {
        var op = await _groupBiz.RequestTimeOff(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("groups/time-offs/{id:guid}/history")]
    public async Task<IActionResult> TimeOffHistory(Guid id, [FromBody] GridFilterWithParams<IdDto> filter)
    {
        var op = await _groupBiz.TimeOffHistory(_identity.User!.UserId, id, filter);
        return Json(op);
    }

    [HttpPost("groups/time-offs/{id:guid}/detail")]
    public async Task<IActionResult> TimeOffDetail(Guid id)
    {
        var op = await _groupBiz.TimeOffDetail(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/time-offs/{id:guid}/approve")]
    public async Task<IActionResult> TimeOffApprove(Guid id)
    {
        var op = await _groupBiz.TimeOffResponse(_identity.User!.UserId, id, true);
        return Json(op);
    }

    [HttpPost("groups/time-offs/{id:guid}/delete")]
    public async Task<IActionResult> RemoveTimeOff(Guid id)
    {
        var op = await _groupBiz.RemoveTimeOff(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/time-offs/{id:guid}/decline")]
    public async Task<IActionResult> TimeOffDecline(Guid id)
    {
        var op = await _groupBiz.TimeOffResponse(_identity.User!.UserId, id, false);
        return Json(op);
    }

    [HttpPost("groups/entry-logs/{id:guid}")]
    public async Task<IActionResult> EntryLogs(Guid id, [FromBody] GridFilter filter)
    {
        var op = await _groupBiz.EntryLogs(_identity.User!.UserId, id, filter);
        return Json(op);
    }

    [HttpPost("groups/toggle-entry/{id:guid}")]
    public async Task<IActionResult> ToggleEntry(Guid id)
    {
        var op = await _groupBiz.ToggleEntry(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/remove-entry/{id:guid}")]
    public async Task<IActionResult> RemoveEntry(Guid id)
    {
        var op = await _groupBiz.RemoveEntry(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/edit-entry/{id:guid}")]
    public async Task<IActionResult> EditEntry(Guid id, [FromBody] OptionalDurationDto model)
    {
        var op = await _groupBiz.EditEntry(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("groups/manual-entry/{id:guid}")]
    public async Task<IActionResult> ManualEntry(Guid id, [FromBody] ManualEntryDto model)
    {
        var op = await _groupBiz.ManualEntry(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("groups/remove-access/{id:guid}")]
    public async Task<IActionResult> RemoveAccess(Guid id)
    {
        var op = await _groupBiz.RemoveAccess(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/change-access/{id:guid}")]
    public async Task<IActionResult> ChangeAccess(Guid id, [FromBody] ChangeAccessDto permission)
    {
        var op = await _groupBiz.ChangeAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("groups/remove-pending-access/{id:guid}")]
    public async Task<IActionResult> RemovePendingAccess(Guid id)
    {
        var op = await _groupBiz.RemovePendingAccess(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/change-pending-access/{id:guid}")]
    public async Task<IActionResult> ChangePendingAccess(Guid id, [FromBody] ChangeAccessDto permission)
    {
        var op = await _groupBiz.ChangePendingAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("groups/export/{id:guid}")]
    public async Task<IActionResult> Export(Guid id)
    {
        var op = await _groupBiz.Export(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/remove")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var op = await _groupBiz.Remove(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var op = await _groupBiz.Archive(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var op = await _groupBiz.Restore(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/fetch")]
    public async Task<IActionResult> Fetch(Guid id)
    {
        var op = await _groupBiz.Fetch(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/report")]
    public async Task<IActionResult> Report(Guid id, [FromBody] DurationDto model)
    {
        var op = await _groupBiz.Report(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("groups/{id:guid}/add-access")]
    public async Task<IActionResult> AddAccess(Guid id, [FromBody] AccessDto permission)
    {
        var op = await _groupBiz.AddAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }
}