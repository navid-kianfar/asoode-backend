using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Communication;
using Asoode.Shared.Endpoint.Extensions.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Communication;


[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Messenger")]
public class MessengerController : BaseController
{
    private readonly IMessengerService _messengerBiz;
    private readonly IUserIdentityService _identity;

    public MessengerController(IMessengerService messengerBiz, IUserIdentityService identity)
    {
        _messengerBiz = messengerBiz;
        _identity = identity;
    }

    [HttpPost("messenger/channels")]
    public async Task<IActionResult> Channels()
    {
        var op = await _messengerBiz.Channels(_identity.User!.UserId);
        return Json(op);
    }

    [HttpPost("messenger/channel/{id:guid}/attach")]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> AddAttachment(Guid id)
    {
        var file = await Request.Form.Files.First().ToStorageItem();
        var op = await _messengerBiz.AddAttachment(_identity.User!.UserId, id, file);
        return Json(op);
    }

    [HttpPost("messenger/channel/{id:guid}/fetch")]
    public async Task<IActionResult> ChannelMessages(Guid id)
    {
        var op = await _messengerBiz.ChannelMessages(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("messenger/channel/{id:guid}/send")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] ChatDto model)
    {
        var op = await _messengerBiz.SendMessage(_identity.User!.UserId, id, model);
        return Json(op);
    }
}