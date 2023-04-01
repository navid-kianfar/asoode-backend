using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.ProjectManagement;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.ProjectManagement;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.ProjectManagement
{
    [JwtAuthorize]
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
            var op = await _projectBiz.List(Identity.UserId);
            return Json(op);
        }

        [HttpPost("create")]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] ProjectCreateViewModel model)
        {
            var op = await _projectBiz.Create(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/archive")]
        [ValidateModel]
        public async Task<IActionResult> Archive(Guid id)
        {
            var op = await _projectBiz.Archive(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/remove")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var op = await _projectBiz.Remove(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("{id:guid}/fetch")]
        public async Task<IActionResult> Fetch(Guid id)
        {
            var op = await _projectBiz.Fetch(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("archived")]
        [ValidateModel]
        public async Task<IActionResult> Archived()
        {
            var op = await _projectBiz.Archived(Identity.UserId);
            return Json(op);
        }
        
        [HttpPost("objectives/{id:guid}")]
        public async Task<IActionResult> Objectives(Guid id)
        {
            var op = await _projectBiz.Objectives(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("objectives/{id:guid}/detail")]
        public async Task<IActionResult> ObjectiveDetails(Guid id)
        {
            var op = await _projectBiz.ObjectiveDetails(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("tree/{id:guid}")]
        public async Task<IActionResult> Tree(Guid id)
        {
            var op = await _projectBiz.Tree(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("progress/{id:guid}")]
        public async Task<IActionResult> ProjectProgress(Guid id)
        {
            var op = await _projectBiz.ProjectProgress(Identity.UserId, id);
            return Json(op);
        }
        
        [HttpPost("road-map/{id:guid}")]
        public async Task<IActionResult> RoadMap(Guid id)
        {
            var op = await _projectBiz.RoadMap(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("export/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Export(Guid id)
        {
            var op = await _projectBiz.Export(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/add-access")]
        [ValidateModel]
        public async Task<IActionResult> AddAccess(Guid id, [FromBody] AccessViewModel permission)
        {
            var op = await _projectBiz.AddAccess(Identity.UserId, id, permission);
            return Json(op);
        }

        [HttpPost("remove-access/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> RemoveAccess(Guid id)
        {
            var op = await _projectBiz.RemoveAccess(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("change-access/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> ChangeAccess(Guid id, [FromBody] ChangeAccessViewModel permission)
        {
            var op = await _projectBiz.ChangeAccess(Identity.UserId, id, permission);
            return Json(op);
        }

        [HttpPost("remove-pending-access/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> RemovePendingAccess(Guid id)
        {
            var op = await _projectBiz.RemovePendingAccess(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("change-pending-access/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> ChangePendingAccess(Guid id, [FromBody] ChangeAccessViewModel permission)
        {
            var op = await _projectBiz.ChangePendingAccess(Identity.UserId, id, permission);
            return Json(op);
        }

        [HttpPost("{id:guid}/sub/create")]
        [ValidateModel]
        public async Task<IActionResult> CreateSubProject(Guid id, [FromBody] CreateSubProjectViewModel model)
        {
            var op = await _projectBiz.CreateSubProject(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("sub/{id:guid}/edit")]
        [ValidateModel]
        public async Task<IActionResult> EditSubProject(Guid id, [FromBody] SimpleViewModel model)
        {
            var op = await _projectBiz.EditSubProject(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("sub/{id:guid}/order")]
        [ValidateModel]
        public async Task<IActionResult> OrderSubProject(Guid id, [FromBody] ChangeOrderViewModel model)
        {
            var op = await _projectBiz.OrderSubProject(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("{id:guid}/edit")]
        [ValidateModel]
        public async Task<IActionResult> EditProject(Guid id, [FromBody] ProjectEditViewModel model)
        {
            var op = await _projectBiz.EditProject(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("sub/{id:guid}/remove")]
        [ValidateModel]
        public async Task<IActionResult> RemoveSubProject(Guid id)
        {
            var op = await _projectBiz.RemoveSubProject(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("{id:guid}/season/create")]
        [ValidateModel]
        public async Task<IActionResult> CreateSeason(Guid id, [FromBody] SimpleViewModel model)
        {
            var op = await _projectBiz.CreateSeason(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("season/{id:guid}/edit")]
        [ValidateModel]
        public async Task<IActionResult> EditSeason(Guid id, [FromBody] SimpleViewModel model)
        {
            var op = await _projectBiz.EditSeason(Identity.UserId, id, model);
            return Json(op);
        }

        [HttpPost("season/{id:guid}/remove")]
        [ValidateModel]
        public async Task<IActionResult> RemoveSeason(Guid id)
        {
            var op = await _projectBiz.RemoveSeason(Identity.UserId, id);
            return Json(op);
        }
    }
}