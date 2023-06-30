using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Storage;


[Route("v2/files")]
[ApiExplorerSettings(GroupName = "Storage")]
public class FileManagerController : BaseController
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IStorageBiz _storageBiz;

    public FileManagerController(IStorageBiz storageBiz, IServiceProvider serviceProvider)
    {
        _storageBiz = storageBiz;
        _serviceProvider = serviceProvider;
    }


    [HttpPost("mine")]
    
    public async Task<IActionResult> Mine([FromBody] FileManagerDto model)
    {
        var op = await _storageBiz.Mine(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("delete")]
    
    public async Task<IActionResult> Delete([FromBody] FileManagerDeleteDto model)
    {
        var op = await _storageBiz.Delete(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("new-folder")]
    
    public async Task<IActionResult> NewFolder([FromBody] FileManagerNameDto model)
    {
        var op = await _storageBiz.NewFolder(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("rename")]
    
    public async Task<IActionResult> Rename([FromBody] FileManagerNameDto model)
    {
        var op = await _storageBiz.Rename(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("upload")]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> Upload()
    {
        var jsonBiz = _serviceProvider.GetService<IJsonBiz>();
        var file = await Request.Form.Files.First().ToStorageItem();
        var model = jsonBiz.Deserialize<FileManagerDto>(Request.Form["data"]);
        var op = await _storageBiz.Upload(_identity.User!.UserId, file, model);
        return Json(op);
    }

    [HttpPost("shared-by-me")]
    
    public async Task<IActionResult> SharedByMe([FromBody] FileManagerDto model)
    {
        var op = await _storageBiz.SharedByMe(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("shared-by-others")]
    
    public async Task<IActionResult> SharedByOthers([FromBody] FileManagerDto model)
    {
        var op = await _storageBiz.SharedByOthers(_identity.User!.UserId, model);
        return Json(op);
    }
}