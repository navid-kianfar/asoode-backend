using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Admin;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Admin;

[JwtAuthorize(UserType.Admin)]
[Route("v2/admin/marketers")]
public class AdminMarketersController : BaseController
{
    private readonly IMarketerBiz _marketerBiz;

    public AdminMarketersController(IMarketerBiz marketerBiz)
    {
        _marketerBiz = marketerBiz;
    }

    [ValidateModel]
    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] GridFilter model)
    {
        var op = await _marketerBiz.AdminList(Identity.UserId, model);
        return Json(op);
    }

    [ValidateModel]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] MarketerEditableViewModel model)
    {
        var op = await _marketerBiz.AdminCreate(Identity.UserId, model);
        return Json(op);
    }

    [ValidateModel]
    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] MarketerEditableViewModel model)
    {
        var op = await _marketerBiz.AdminEdit(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var op = await _marketerBiz.AdminDelete(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("toggle/{id:guid}")]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var op = await _marketerBiz.AdminToggle(Identity.UserId, id);
        return Json(op);
    }
}