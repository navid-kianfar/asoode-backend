using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Extensions;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Import;
using Asoode.Core.ViewModels.Import.Trello;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Asoode.Backend.Controllers.Import;

[JwtAuthorize]
[Route("v2/import")]
[ApiExplorerSettings(GroupName = "Import")]
public class ImportController : BaseController
{
    private readonly IServiceProvider _serviceProvider;

    public ImportController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [Route("trello")]
    [HttpPost]
    public async Task<IActionResult> Trello()
    {
        var file = await Request.Form.Files.First().ToStorageItem();
        var data = JsonConvert.DeserializeObject<TrelloMapedDataViewModel>(Request.Form["data"]);
        var op = await _serviceProvider
            .GetService<ITrelloBiz>()
            .Import(file, data, Identity.UserId);
        return Json(op);
    }

    [Route("taskulu")]
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Taskulu()
    {
        var file = await Request.Form.Files.First().ToStorageItem();
        var op = await _serviceProvider.GetService<ITaskuluBiz>()
            .Import(file, Identity.UserId);
        return Json(op);
    }
}