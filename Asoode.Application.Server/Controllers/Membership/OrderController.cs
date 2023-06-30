using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Membership.Order;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Membership;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Orders")]
public class OrderController : BaseController
{
    private readonly IOrderService _orderBiz;
    private readonly IUserIdentityService _identity;

    public OrderController(IOrderService orderBiz, IUserIdentityService identity)
    {
        _orderBiz = orderBiz;
        _identity = identity;
    }

    
    [HttpPost("orders/check-discount")]
    
    public async Task<IActionResult> CheckDiscount([FromBody] CheckDiscountDto model)
    {
        var op = await _orderBiz.CheckDiscount(_identity.User!.UserId, model);
        return Json(op);
    }

    
    [HttpPost("orders/new")]
    
    public async Task<IActionResult> Order([FromBody] RequestOrderDto model)
    {
        var op = await _orderBiz.Order(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpGet("orders/pay/{id:guid}")]
    public async Task<IActionResult> Pay(Guid id)
    {
        var op = await _orderBiz.Pay(id);
        return Redirect(op.Data);
    }

    [HttpGet("orders/pdf/{id:guid}")]
    public async Task<IActionResult> Pdf(Guid id)
    {
        Stream? stream = await _orderBiz.Pdf(id);
        if (stream == null) return NotFound();
        return File(stream, "application/pdf", id + ".pdf");
    }
}