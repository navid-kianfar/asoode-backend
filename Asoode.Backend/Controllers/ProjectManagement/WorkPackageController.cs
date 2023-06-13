using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.ProjectManagement;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.ProjectManagement;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.ProjectManagement;

[JwtAuthorize]
[Route("v2/work-packages")]
[ApiExplorerSettings(GroupName = "Work Package")]
public class WorkPackageController : BaseController
{
    private readonly IWorkPackageBiz _workPackageBiz;

    public WorkPackageController(IWorkPackageBiz workPackageBiz)
    {
        _workPackageBiz = workPackageBiz;
    }

    [HttpPost("labels/{id:guid}/rename")]
    [ValidateModel]
    public async Task<IActionResult> RenameLabel(Guid id, [FromBody] LabelViewModel model)
    {
        var op = await _workPackageBiz.RenameLabel(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("labels/{id:guid}/remove")]
    [ValidateModel]
    public async Task<IActionResult> RemoveLabel(Guid id)
    {
        var op = await _workPackageBiz.RemoveLabel(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("labels/{id:guid}/create")]
    [ValidateModel]
    public async Task<IActionResult> CreateLabel(Guid id, [FromBody] LabelViewModel model)
    {
        var op = await _workPackageBiz.CreateLabel(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("create/{id:guid}")]
    [ValidateModel]
    public async Task<IActionResult> CreateWorkPackage(Guid id, [FromBody] CreateWorkPackageViewModel model)
    {
        var op = await _workPackageBiz.CreateWorkPackage(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("fetch/{id:guid}")]
    public async Task<IActionResult> Fetch(Guid id, [FromBody] WorkPackageFilterViewModel filter)
    {
        var op = await _workPackageBiz
            .Fetch(Identity.UserId, id, filter);
        return Json(op);
    }

    [HttpPost("fetch/{id:guid}/archived")]
    public async Task<IActionResult> FetchArchived(Guid id, [FromBody] WorkPackageFilterViewModel filter)
    {
        var op = await _workPackageBiz
            .FetchArchived(Identity.UserId, id, filter);
        return Json(op);
    }

    [HttpPost("{id:guid}/remove")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var op = await _workPackageBiz.Remove(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/add-access")]
    [ValidateModel]
    public async Task<IActionResult> AddAccess(Guid id, [FromBody] AccessViewModel permission)
    {
        var op = await _workPackageBiz.AddAccess(Identity.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("{id:guid}/permissions")]
    [ValidateModel]
    public async Task<IActionResult> Permissions(Guid id, [FromBody] WorkPackagePermissionViewModel permission)
    {
        var op = await _workPackageBiz.Permissions(Identity.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("{id:guid}/order")]
    [ValidateModel]
    public async Task<IActionResult> Order(Guid id, [FromBody] ChangeOrderViewModel model)
    {
        var op = await _workPackageBiz.Order(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/sort-orders")]
    [ValidateModel]
    public async Task<IActionResult> SortOrder(Guid id, [FromBody] SortOrderViewModel model)
    {
        var op = await _workPackageBiz.SortOrder(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/edit")]
    [ValidateModel]
    public async Task<IActionResult> Edit(Guid id, [FromBody] SimpleViewModel model)
    {
        var op = await _workPackageBiz.Edit(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/setting")]
    [ValidateModel]
    public async Task<IActionResult> Setting(Guid id, [FromBody] WorkPackageSettingViewModel model)
    {
        var op = await _workPackageBiz.Setting(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/upgrade")]
    [ValidateModel]
    public async Task<IActionResult> Upgrade(Guid id)
    {
        var op = await _workPackageBiz.Upgrade(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/connect/{projectId:guid}")]
    [ValidateModel]
    public async Task<IActionResult> Connect(Guid id, Guid projectId)
    {
        var op = await _workPackageBiz.Connect(Identity.UserId, id, projectId);
        return Json(op);
    }

    [HttpPost("{id:guid}/merge/{destinationId:guid}")]
    [ValidateModel]
    public async Task<IActionResult> Merge(Guid id, Guid destinationId)
    {
        var op = await _workPackageBiz.Merge(Identity.UserId, id, destinationId);
        return Json(op);
    }

    [HttpPost("{id:guid}/setting/user")]
    [ValidateModel]
    public async Task<IActionResult> UserSetting(Guid id, [FromBody] WorkPackageUserSettingViewModel model)
    {
        var op = await _workPackageBiz.UserSetting(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/leave")]
    [ValidateModel]
    public async Task<IActionResult> Leave(Guid id)
    {
        var op = await _workPackageBiz.Leave(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/archive")]
    [ValidateModel]
    public async Task<IActionResult> Archive(Guid id)
    {
        var op = await _workPackageBiz.Archive(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("remove-access/{id:guid}")]
    [ValidateModel]
    public async Task<IActionResult> RemoveAccess(Guid id)
    {
        var op = await _workPackageBiz.RemoveAccess(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("change-access/{id:guid}")]
    [ValidateModel]
    public async Task<IActionResult> ChangeAccess(Guid id, [FromBody] ChangeAccessViewModel permission)
    {
        var op = await _workPackageBiz.ChangeAccess(Identity.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("remove-pending-access/{id:guid}")]
    [ValidateModel]
    public async Task<IActionResult> RemovePendingAccess(Guid id)
    {
        var op = await _workPackageBiz.RemovePendingAccess(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("change-pending-access/{id:guid}")]
    [ValidateModel]
    public async Task<IActionResult> ChangePendingAccess(Guid id, [FromBody] ChangeAccessViewModel permission)
    {
        var op = await _workPackageBiz.ChangePendingAccess(Identity.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("{id:guid}/lists/create")]
    [ValidateModel]
    public async Task<IActionResult> CreateList(Guid id, [FromBody] TitleViewModel model)
    {
        var op = await _workPackageBiz.CreateList(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/objectives/create")]
    [ValidateModel]
    public async Task<IActionResult> CreateObjective(Guid id, [FromBody] CreateObjectiveViewModel model)
    {
        var op = await _workPackageBiz.CreateObjective(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("objectives/{id:guid}/edit")]
    [ValidateModel]
    public async Task<IActionResult> EditObjective(Guid id, [FromBody] CreateObjectiveViewModel model)
    {
        var op = await _workPackageBiz.EditObjective(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("objectives/{id:guid}/delete")]
    [ValidateModel]
    public async Task<IActionResult> DeleteObjective(Guid id)
    {
        var op = await _workPackageBiz.DeleteObjective(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("lists/{id:guid}/clone")]
    [ValidateModel]
    public async Task<IActionResult> CloneList(Guid id, [FromBody] TitleViewModel model)
    {
        var op = await _workPackageBiz.CloneList(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("lists/{id:guid}/archive")]
    [ValidateModel]
    public async Task<IActionResult> ArchiveList(Guid id)
    {
        var op = await _workPackageBiz.ArchiveList(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("lists/{id:guid}/archive-tasks")]
    public async Task<IActionResult> ArchiveListTasks(Guid id)
    {
        var op = await _workPackageBiz.ArchiveListTasks(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("lists/{id:guid}/clear-tasks")]
    public async Task<IActionResult> DeleteListTasks(Guid id)
    {
        var op = await _workPackageBiz.DeleteListTasks(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("lists/{id:guid}/reposition")]
    [ValidateModel]
    public async Task<IActionResult> RepositionList(Guid id, [FromBody] RepositionViewModel model)
    {
        var op = await _workPackageBiz.RepositionList(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("lists/{id:guid}/rename")]
    [ValidateModel]
    public async Task<IActionResult> RenameList(Guid id, [FromBody] TitleViewModel model)
    {
        var op = await _workPackageBiz.RenameList(Identity.UserId, id, model);
        return Json(op);
    }
}