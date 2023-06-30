using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.ProjectManagement;


[Route("v2/projects")]
[ApiExplorerSettings(GroupName = "Project")]
public class ProjectController : BaseController
{
    private readonly IProjectBiz _projectBiz;

    public ProjectController(IProjectBiz projectBiz)
    {
        _projectBiz = projectBiz;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List()
    {
        var op = await _projectBiz.List(_identity.User!.UserId);
        return Json(op);
    }

    [HttpPost("create")]
    
    public async Task<IActionResult> Create([FromBody] ProjectCreateDto model)
    {
        var op = await _projectBiz.Create(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/archive")]
    
    public async Task<IActionResult> Archive(Guid id)
    {
        var op = await _projectBiz.Archive(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/remove")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var op = await _projectBiz.Remove(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/fetch")]
    public async Task<IActionResult> Fetch(Guid id)
    {
        var op = await _projectBiz.Fetch(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("archived")]
    
    public async Task<IActionResult> Archived()
    {
        var op = await _projectBiz.Archived(_identity.User!.UserId);
        return Json(op);
    }

    [HttpPost("objectives/{id:guid}")]
    public async Task<IActionResult> Objectives(Guid id)
    {
        var op = await _projectBiz.Objectives(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("objectives/{id:guid}/detail")]
    public async Task<IActionResult> ObjectiveDetails(Guid id)
    {
        var op = await _projectBiz.ObjectiveDetails(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("tree/{id:guid}")]
    public async Task<IActionResult> Tree(Guid id)
    {
        var op = await _projectBiz.Tree(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("progress/{id:guid}")]
    public async Task<IActionResult> ProjectProgress(Guid id)
    {
        var op = await _projectBiz.ProjectProgress(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("road-map/{id:guid}")]
    public async Task<IActionResult> RoadMap(Guid id)
    {
        var op = await _projectBiz.RoadMap(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("export/{id:guid}")]
    
    public async Task<IActionResult> Export(Guid id)
    {
        var op = await _projectBiz.Export(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/add-access")]
    
    public async Task<IActionResult> AddAccess(Guid id, [FromBody] AccessDto permission)
    {
        var op = await _projectBiz.AddAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("remove-access/{id:guid}")]
    
    public async Task<IActionResult> RemoveAccess(Guid id)
    {
        var op = await _projectBiz.RemoveAccess(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("change-access/{id:guid}")]
    
    public async Task<IActionResult> ChangeAccess(Guid id, [FromBody] ChangeAccessDto permission)
    {
        var op = await _projectBiz.ChangeAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("remove-pending-access/{id:guid}")]
    
    public async Task<IActionResult> RemovePendingAccess(Guid id)
    {
        var op = await _projectBiz.RemovePendingAccess(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("change-pending-access/{id:guid}")]
    
    public async Task<IActionResult> ChangePendingAccess(Guid id, [FromBody] ChangeAccessDto permission)
    {
        var op = await _projectBiz.ChangePendingAccess(_identity.User!.UserId, id, permission);
        return Json(op);
    }

    [HttpPost("{id:guid}/sub/create")]
    
    public async Task<IActionResult> CreateSubProject(Guid id, [FromBody] CreateSubProjectDto model)
    {
        var op = await _projectBiz.CreateSubProject(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("sub/{id:guid}/edit")]
    
    public async Task<IActionResult> EditSubProject(Guid id, [FromBody] SimpleDto model)
    {
        var op = await _projectBiz.EditSubProject(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("sub/{id:guid}/order")]
    
    public async Task<IActionResult> OrderSubProject(Guid id, [FromBody] ChangeOrderDto model)
    {
        var op = await _projectBiz.OrderSubProject(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/edit")]
    
    public async Task<IActionResult> EditProject(Guid id, [FromBody] ProjectEditDto model)
    {
        var op = await _projectBiz.EditProject(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("sub/{id:guid}/remove")]
    
    public async Task<IActionResult> RemoveSubProject(Guid id)
    {
        var op = await _projectBiz.RemoveSubProject(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/season/create")]
    
    public async Task<IActionResult> CreateSeason(Guid id, [FromBody] SimpleDto model)
    {
        var op = await _projectBiz.CreateSeason(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("season/{id:guid}/edit")]
    
    public async Task<IActionResult> EditSeason(Guid id, [FromBody] SimpleDto model)
    {
        var op = await _projectBiz.EditSeason(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("season/{id:guid}/remove")]
    
    public async Task<IActionResult> RemoveSeason(Guid id)
    {
        var op = await _projectBiz.RemoveSeason(_identity.User!.UserId, id);
        return Json(op);
    }
}