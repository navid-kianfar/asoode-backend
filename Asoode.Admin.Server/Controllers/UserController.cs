using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "User")]
public class UserController : BaseController
{
    private readonly IAccountService _accountBiz;
    private readonly IUserIdentityService _identity;

    public UserController(IAccountService accountBiz, IUserIdentityService identity)
    {
        _accountBiz = accountBiz;
        _identity = identity;
    }


    [HttpPost("user/list")]
    public async Task<IActionResult> List([FromBody] GridFilterWithParams<GridQuery> model)
    {
        var op = await _accountBiz.List(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("user/confirm/{id:guid}")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var op = await _accountBiz.ConfirmUser(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("user/block/{id:guid}")]
    public async Task<IActionResult> Block(Guid id)
    {
        var op = await _accountBiz.BlockUser(_identity.User!.UserId, id);
        return Json(op);
    }


    [HttpPost("user/edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] UserEditDto model)
    {
        var op = await _accountBiz.EditUser(_identity.User!.UserId, id, model);
        return Json(op);
    }


    [HttpPost("user/reset-password/{id:guid}")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] UserResetPasswordDto model)
    {
        var op = await _accountBiz.ResetUserPassword(_identity.User!.UserId, id, model);
        return Json(op);
    }
}