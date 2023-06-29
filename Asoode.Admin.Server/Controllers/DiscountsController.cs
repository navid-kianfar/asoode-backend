using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Discount;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Discounts")]
public class DiscountsController : BaseController
{
    private readonly IDiscountService _discountBiz;
    private readonly IUserIdentityService _identity;

    public DiscountsController(IDiscountService discountBiz, IUserIdentityService identity)
    {
        _discountBiz = discountBiz;
        _identity = identity;
    }


    [HttpPost("discounts/list")]
    public async Task<IActionResult> List([FromBody] GridFilter model)
    {
        var op = await _discountBiz.List(_identity.User!.UserId, model);
        return Json(op);
    }


    [HttpPost("discounts/create")]
    public async Task<IActionResult> Create([FromBody] DiscountEditableDto model)
    {
        var op = await _discountBiz.Create(_identity.User!.UserId, model);
        return Json(op);
    }


    [HttpPost("discounts/edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] DiscountEditableDto model)
    {
        var op = await _discountBiz.Edit(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("discounts/delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var op = await _discountBiz.Delete(_identity.User!.UserId, id);
        return Json(op);
    }
}