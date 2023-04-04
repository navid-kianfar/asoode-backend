using Asoode.Application.Core.Contracts.Communication;
using Asoode.Application.Core.ViewModels.Communication;
using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Extensions;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.Communication
{
    [JwtAuthorize]
    [Route("v2/messenger")]
    [ApiExplorerSettings(GroupName = "Messenger")]
    public class MessengerController : BaseController
    {
        private readonly IMessengerBiz _messengerBiz;

        public MessengerController(IMessengerBiz messengerBiz)
        {
            _messengerBiz = messengerBiz;
        }

        [HttpPost("channels")]
        public async Task<IActionResult> Channels()
        {
            var op = await _messengerBiz.Channels(Identity.UserId);
            return Json(op);
        }

        [HttpPost("channel/{id:guid}/attach")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> AddAttachment(Guid id)
        {
            var file = await Request.Form.Files?.FirstOrDefault()?.ToViewModel();
            var op = await _messengerBiz.AddAttachment(Identity.UserId, id, file);
            return Json(op);
        }
        
        [HttpPost("channel/{id:guid}/fetch")]
        public async Task<IActionResult> ChannelMessages(Guid id)
        {
            var op = await _messengerBiz.ChannelMessages(Identity.UserId, id);
            return Json(op);
        }

        [HttpPost("channel/{id:guid}/send")]
        public async Task<IActionResult> SendMessage(Guid id, [FromBody] ChatViewModel model)
        {
            var op = await _messengerBiz.SendMessage(Identity.UserId, id, model);
            return Json(op);
        }
    }
}