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
[Route("v2/admin/discounts")]
public class AdminDiscountsController : BaseController
{
    private readonly IDiscountBiz _discountBiz;

    public AdminDiscountsController(IDiscountBiz discountBiz)
    {
        _discountBiz = discountBiz;
    }

    [ValidateModel]
    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] GridFilter model)
    {
        var op = await _discountBiz.AdminList(Identity.UserId, model);
        return Json(op);
    }

    [ValidateModel]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] DiscountEditableViewModel model)
    {
        var op = await _discountBiz.AdminCreate(Identity.UserId, model);
        return Json(op);
    }

    [ValidateModel]
    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] DiscountEditableViewModel model)
    {
        var op = await _discountBiz.AdminEdit(Identity.UserId, id, model);
        return Json(op);
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var op = await _discountBiz.AdminDelete(Identity.UserId, id);
        return Json(op);
    }
}