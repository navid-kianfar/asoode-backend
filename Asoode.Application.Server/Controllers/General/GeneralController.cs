using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Contact;
using Asoode.Shared.Abstraction.Dtos.General.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.General;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "General")]
public class GeneralController : BaseController
{
    private readonly IServiceProvider _serviceProvider;

    public GeneralController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [AllowAnonymous]
    [HttpGet("enums")]
    public IActionResult Enums()
    {
        var result = _serviceProvider.GetService<IGeneralService>()!.Enums();
        return Json(result);
    }

    
    
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRequestDto model)
    {
        var identity = _serviceProvider.GetService<IUserIdentityService>()!;
        var op = await _serviceProvider.GetService<ISearchService>()!
            .Query(model.Search, identity.User!.UserId);
        return Json(op);
    }

    
    // [HttpPost("contact")]
    // public async Task<IActionResult> Contact([FromBody] ContactDto model)
    // {
    //     var op = await _serviceProvider.GetService<IContactService>().Contact(model);
    //     return Json(op);
    // }
}