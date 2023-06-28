using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[Route(EndpointConstants.Prefix)]
public class ErrorsController : BaseController
{
    private readonly IErrorBiz _errorBiz;
    private readonly IUserIdentityService _identity;

    public ErrorsController(IErrorBiz errorBiz, IUserIdentityService identity)
    {
        _errorBiz = errorBiz;
        _identity = identity;
    }


    [HttpPost("errors/list")]
    public async Task<IActionResult> List([FromBody] GridFilter model)
    {
        var op = await _errorBiz.List(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("errors/delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var op = await _errorBiz.Delete(_identity.User!.UserId, id);
        return Json(op);
    }
}