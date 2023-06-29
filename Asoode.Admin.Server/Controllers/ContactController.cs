using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Contact")]
public class ContactController : BaseController
{
    private readonly IContactService _contactBiz;
    private readonly IUserIdentityService _identity;

    public ContactController(IContactService contactBiz, IUserIdentityService identity)
    {
        _contactBiz = contactBiz;
        _identity = identity;
    }


    [HttpPost("contact/list")]
    public async Task<IActionResult> List([FromBody] GridFilterWithParams<GridQuery> model)
    {
        var op = await _contactBiz.List(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("contact/delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var op = await _contactBiz.Delete(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("contact/replies/{id:guid}")]
    public async Task<IActionResult> Replies(Guid id)
    {
        var op = await _contactBiz.Replies(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("contact/reply/{id:guid}")]
    public async Task<IActionResult> Reply(Guid id, [FromBody] ContactReplyDto model)
    {
        var op = await _contactBiz.Reply(_identity.User!.UserId, id, model);
        return Json(op);
    }
}