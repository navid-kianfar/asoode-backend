using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Membership;

[Route("v2/orders")]
public class OrderController : BaseController
{
    private readonly IOrderBiz _orderBiz;

    public OrderController(IOrderBiz orderBiz)
    {
        _orderBiz = orderBiz;
    }

    
    [HttpPost("check-discount")]
    
    public async Task<IActionResult> CheckDiscount([FromBody] CheckDiscountDto model)
    {
        var op = await _orderBiz.CheckDiscount(_identity.User!.UserId, model);
        return Json(op);
    }

    
    [HttpPost("new")]
    
    public async Task<IActionResult> Order([FromBody] RequestOrderDto model)
    {
        var op = await _orderBiz.Order(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpGet("pay/{id:guid}")]
    public async Task<IActionResult> Pay(Guid id)
    {
        var op = await _orderBiz.Pay(id);
        return Redirect(op.Data);
    }

    [HttpGet("pdf/{id:guid}")]
    public async Task<IActionResult> Pdf(Guid id)
    {
        var stream = await _orderBiz.Pdf(id);
        if (stream == null) return NotFound();
        return File(stream, "application/pdf", id + ".pdf");
    }

    [HttpGet("pay-ping-callback")]
    public async Task<IActionResult> PayPingCallBack()
    {
        var transId = "";
        var paymentId = Guid.Empty;
        if (Microsoft.AspNetCore.Http.HttpContext.Request.QueryString.HasValue)
        {
            transId = Microsoft.AspNetCore.Http.HttpContext.Request.Query["refid"].ToString();
            var clientId = Microsoft.AspNetCore.Http.HttpContext.Request.Query["clientrefid"].ToString();
            Guid.TryParse(clientId, out paymentId);
        }

        var op = await _orderBiz.PayPingCallBack(transId, paymentId);
        return Redirect(op.Data.Item2);
    }
}