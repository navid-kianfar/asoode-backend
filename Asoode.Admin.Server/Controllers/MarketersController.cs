using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Marketer;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Marketers")]
public class MarketersController : BaseController
{
    private readonly IUserIdentityService _identity;
    private readonly IMarketerService _marketerBiz;

    public MarketersController(IMarketerService marketerBiz, IUserIdentityService identity)
    {
        _marketerBiz = marketerBiz;
        _identity = identity;
    }


    [HttpPost("marketers/list")]
    public async Task<IActionResult> List([FromBody] GridFilter model)
    {
        var op = await _marketerBiz.List(_identity.User!.UserId, model);
        return Json(op);
    }


    [HttpPost("marketers/create")]
    public async Task<IActionResult> Create([FromBody] MarketerEditableDto model)
    {
        var op = await _marketerBiz.Create(_identity.User!.UserId, model);
        return Json(op);
    }


    [HttpPost("marketers/edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] MarketerEditableDto model)
    {
        var op = await _marketerBiz.Edit(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("marketers/delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var op = await _marketerBiz.Delete(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("marketers/toggle/{id:guid}")]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var op = await _marketerBiz.Toggle(_identity.User!.UserId, id);
        return Json(op);
    }
}