using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Membership;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Admin;

[JwtAuthorize(UserType.Admin)]
[Route("v2/admin/user")]
public class UserController : BaseController
{
    private readonly IAccountBiz _accountBiz;

    public UserController(IAccountBiz accountBiz)
    {
        _accountBiz = accountBiz;
    }

    [ValidateModel]
    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] GridFilterWithParams<GridQuery> model)
    {
        var op = await _accountBiz.AdminUsersList(Identity.UserId, model);
        return Json(op);
    }

    [HttpPost("confirm/{id:guid}")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var op = await _accountBiz.AdminConfirmUser(Identity.UserId, id);
        return Json(op);
    }

    [HttpPost("block/{id:guid}")]
    public async Task<IActionResult> Block(Guid id)
    {
        var op = await _accountBiz.AdminBlockUser(Identity.UserId, id);
        return Json(op);
    }

    [ValidateModel]
    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] UserEditViewModel model)
    {
        var op = await _accountBiz.AdminEditUser(Identity.UserId, id, model);
        return Json(op);
    }

    [ValidateModel]
    [HttpPost("reset-password/{id:guid}")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] UserResetPasswordViewModel model)
    {
        var op = await _accountBiz.AdminResetUserPassword(Identity.UserId, id, model);
        return Json(op);
    }
}