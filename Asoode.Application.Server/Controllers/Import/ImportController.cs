using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Asoode.Application.Server.Controllers.Import;


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
        var data = JsonConvert.DeserializeObject<TrelloMapedDataDto>(Request.Form["data"]);
        var op = await _serviceProvider
            .GetService<ITrelloBiz>()
            .Import(file, data, _identity.User!.UserId);
        return Json(op);
    }

    [Route("taskulu")]
    [HttpPost]
    
    public async Task<IActionResult> Taskulu()
    {
        var file = await Request.Form.Files.First().ToStorageItem();
        var op = await _serviceProvider.GetService<ITaskuluBiz>()
            .Import(file, _identity.User!.UserId);
        return Json(op);
    }
}