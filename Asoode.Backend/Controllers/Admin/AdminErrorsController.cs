using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Admin;

[JwtAuthorize(UserType.Admin)]
[Route("v2/admin/errors")]
public class AdminErrorsController : BaseController
{
    private readonly IErrorBiz _errorBiz;

    public AdminErrorsController(IErrorBiz errorBiz)
    {
        _errorBiz = errorBiz;
    }

    [ValidateModel]
    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] GridFilter model)
    {
        var op = await _errorBiz.AdminErrorsList(Identity.UserId, model);
        return Json(op);
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var op = await _errorBiz.AdminErrorsDelete(Identity.UserId, id);
        return Json(op);
    }
}