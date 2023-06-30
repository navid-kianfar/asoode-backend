using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.General;

[Route("v2")]
[ApiExplorerSettings(GroupName = "General")]
public class GeneralController : BaseController
{
    private readonly IServiceProvider _serviceProvider;

    public GeneralController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpGet("enums")]
    public IActionResult Enums()
    {
        var result = _serviceProvider.GetService<IGeneralBiz>().Enums();
        return Json(result);
    }

    
    
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRequestDto model)
    {
        var op = await _serviceProvider.GetService<ISearchBiz>()
            .Query(model.Search, _identity.User!.UserId);
        return Json(op);
    }

    
    [HttpPost("contact")]
    public async Task<IActionResult> Contact([FromBody] ContactDto model)
    {
        var op = await _serviceProvider.GetService<IContactBiz>().Contact(model);
        return Json(op);
    }
}