using System;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Admin;
using Asoode.Core.Contracts.General;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.General.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Backend.Controllers.General;

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

    [JwtAuthorize]
    [ValidateModel]
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRequestViewModel model)
    {
        var op = await _serviceProvider.GetService<ISearchBiz>()
            .Query(model.Search, Identity.UserId);
        return Json(op);
    }

    [ValidateModel]
    [HttpPost("contact")]
    public async Task<IActionResult> Contact([FromBody] ContactViewModel model)
    {
        var op = await _serviceProvider.GetService<IContactBiz>().Contact(model);
        return Json(op);
    }
}