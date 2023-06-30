using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.ProjectManagement;


[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Work Package")]
public class WorkPackageController : BaseController
{
    private readonly IWorkPackageService _workPackageBiz;
    private readonly IUserIdentityService _identity;

    public WorkPackageController(IWorkPackageService workPackageBiz, IUserIdentityService identity)
    {
        _workPackageBiz = workPackageBiz;
        _identity = identity;
    }

    [HttpPost("work-packages/labels/{id:guid}/rename")]
    
    public async Task<IActionResult> RenameLabel(Guid id, [FromBody] LabelDto model)
    {
        var op = await _workPackageBiz.RenameLabel(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/labels/{id:guid}/remove")]
    
    public async Task<IActionResult> RemoveLabel(Guid id)
    {
        var op = await _workPackageBiz.RemoveLabel(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/labels/{id:guid}/create")]
    
    public async Task<IActionResult> CreateLabel(Guid id, [FromBody] LabelDto model)
    {
        var op = await _workPackageBiz.CreateLabel(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/create/{id:guid}")]
    
    public async Task<IActionResult> CreateWorkPackage(Guid id, [FromBody] CreateWorkPackageDto model)
    {
        var op = await _workPackageBiz.CreateWorkPackage(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/fetch/{id:guid}")]
    public async Task<IActionResult> Fetch(Guid id, [FromBody] WorkPackageFilterDto filter)
    {
        var op = await _workPackageBiz
            .Fetch(_identity.User!.UserId, id, filter);
        return Json(op);
    }

    [HttpPost("work-packages/fetch/{id:guid}/archived")]
    public async Task<IActionResult> FetchArchived(Guid id, [FromBody] WorkPackageFilterDto filter)
    {
        var op = await _workPackageBiz
            .FetchArchived(_identity.User!.UserId, id, filter);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/remove")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var op = await _workPackageBiz.Remove(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/add-access")]
    
    public async Task<IActionResult> AddAccess(Guid id, [FromBody] AccessDto permission)
    {
        var op = await _workPackageBiz.AddAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/permissions")]
    
    public async Task<IActionResult> Permissions(Guid id, [FromBody] WorkPackagePermissionDto permission)
    {
        var op = await _workPackageBiz.Permissions(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/order")]
    
    public async Task<IActionResult> Order(Guid id, [FromBody] ChangeOrderDto model)
    {
        var op = await _workPackageBiz.Order(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/sort-orders")]
    
    public async Task<IActionResult> SortOrder(Guid id, [FromBody] SortOrderDto model)
    {
        var op = await _workPackageBiz.SortOrder(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/edit")]
    
    public async Task<IActionResult> Edit(Guid id, [FromBody] SimpleDto model)
    {
        var op = await _workPackageBiz.Edit(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/setting")]
    
    public async Task<IActionResult> Setting(Guid id, [FromBody] WorkPackageSettingDto model)
    {
        var op = await _workPackageBiz.Setting(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/upgrade")]
    
    public async Task<IActionResult> Upgrade(Guid id)
    {
        var op = await _workPackageBiz.Upgrade(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/connect/{projectId:guid}")]
    
    public async Task<IActionResult> Connect(Guid id, Guid projectId)
    {
        var op = await _workPackageBiz.Connect(_identity.User!.UserId, id, projectId);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/merge/{destinationId:guid}")]
    
    public async Task<IActionResult> Merge(Guid id, Guid destinationId)
    {
        var op = await _workPackageBiz.Merge(_identity.User!.UserId, id, destinationId);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/setting/user")]
    
    public async Task<IActionResult> UserSetting(Guid id, [FromBody] WorkPackageUserSettingDto model)
    {
        var op = await _workPackageBiz.UserSetting(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/leave")]
    
    public async Task<IActionResult> Leave(Guid id)
    {
        var op = await _workPackageBiz.Leave(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/archive")]
    
    public async Task<IActionResult> Archive(Guid id)
    {
        var op = await _workPackageBiz.Archive(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/remove-access/{id:guid}")]
    
    public async Task<IActionResult> RemoveAccess(Guid id)
    {
        var op = await _workPackageBiz.RemoveAccess(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/change-access/{id:guid}")]
    
    public async Task<IActionResult> ChangeAccess(Guid id, [FromBody] ChangeAccessDto permission)
    {
        var op = await _workPackageBiz.ChangeAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("work-packages/remove-pending-access/{id:guid}")]
    
    public async Task<IActionResult> RemovePendingAccess(Guid id)
    {
        var op = await _workPackageBiz.RemovePendingAccess(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/change-pending-access/{id:guid}")]
    
    public async Task<IActionResult> ChangePendingAccess(Guid id, [FromBody] ChangeAccessDto permission)
    {
        var op = await _workPackageBiz.ChangePendingAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/lists/create")]
    
    public async Task<IActionResult> CreateList(Guid id, [FromBody] TitleDto model)
    {
        var op = await _workPackageBiz.CreateList(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/{id:guid}/objectives/create")]
    
    public async Task<IActionResult> CreateObjective(Guid id, [FromBody] CreateObjectiveDto model)
    {
        var op = await _workPackageBiz.CreateObjective(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/objectives/{id:guid}/edit")]
    
    public async Task<IActionResult> EditObjective(Guid id, [FromBody] CreateObjectiveDto model)
    {
        var op = await _workPackageBiz.EditObjective(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/objectives/{id:guid}/delete")]
    
    public async Task<IActionResult> DeleteObjective(Guid id)
    {
        var op = await _workPackageBiz.DeleteObjective(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/lists/{id:guid}/clone")]
    
    public async Task<IActionResult> CloneList(Guid id, [FromBody] TitleDto model)
    {
        var op = await _workPackageBiz.CloneList(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/lists/{id:guid}/archive")]
    
    public async Task<IActionResult> ArchiveList(Guid id)
    {
        var op = await _workPackageBiz.ArchiveList(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/lists/{id:guid}/archive-tasks")]
    public async Task<IActionResult> ArchiveListTasks(Guid id)
    {
        var op = await _workPackageBiz.ArchiveListTasks(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/lists/{id:guid}/clear-tasks")]
    public async Task<IActionResult> DeleteListTasks(Guid id)
    {
        var op = await _workPackageBiz.DeleteListTasks(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("work-packages/lists/{id:guid}/reposition")]
    
    public async Task<IActionResult> RepositionList(Guid id, [FromBody] RepositionDto model)
    {
        var op = await _workPackageBiz.RepositionList(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("work-packages/lists/{id:guid}/rename")]
    
    public async Task<IActionResult> RenameList(Guid id, [FromBody] TitleDto model)
    {
        var op = await _workPackageBiz.RenameList(_identity.User!.UserId, id, model);
        return Json(op);
    }
}