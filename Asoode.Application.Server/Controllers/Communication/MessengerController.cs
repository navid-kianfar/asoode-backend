using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Communication;


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
        var op = await _messengerBiz.Channels(_identity.User!.UserId);
        return Json(op);
    }

    [HttpPost("channel/{id:guid}/attach")]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> AddAttachment(Guid id)
    {
        var file = await Request.Form.Files.First().ToStorageItem();
        var op = await _messengerBiz.AddAttachment(_identity.User!.UserId, id, file);
        return Json(op);
    }

    [HttpPost("channel/{id:guid}/fetch")]
    public async Task<IActionResult> ChannelMessages(Guid id)
    {
        var op = await _messengerBiz.ChannelMessages(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("channel/{id:guid}/send")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] ChatDto model)
    {
        var op = await _messengerBiz.SendMessage(_identity.User!.UserId, id, model);
        return Json(op);
    }
}