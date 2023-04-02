using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.Membership
{
    [Route("v2/orders")]
    public class OrderController : BaseController
    {
        private readonly IOrderBiz _orderBiz;

        public OrderController(IOrderBiz orderBiz)
        {
            _orderBiz = orderBiz;
        }
        
        [JwtAuthorize]
        [HttpPost("check-discount")]
        [ValidateModel]
        public async Task<IActionResult> CheckDiscount([FromBody]CheckDiscountViewModel model)
        {
            var op = await _orderBiz.CheckDiscount(Identity.UserId, model);
            return Json(op);
        }
        
        [JwtAuthorize]
        [HttpPost("new")]
        [ValidateModel]
        public async Task<IActionResult> Order([FromBody]RequestOrderViewModel model)
        {
            var op = await _orderBiz.Order(Identity.UserId, model);
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
            string transId = "";
            Guid paymentId = Guid.Empty;
            if (HttpContext.Request.QueryString.HasValue)
            {
                transId = HttpContext.Request.Query["refid"].ToString();
                var clientId = HttpContext.Request.Query["clientrefid"].ToString();
                Guid.TryParse(clientId, out paymentId);
            }
            
            var op = await _orderBiz.PayPingCallBack(transId, paymentId);
            return Redirect(op.Data.Item2);
        }
    }
}