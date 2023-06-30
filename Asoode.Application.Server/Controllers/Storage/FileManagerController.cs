using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Endpoint.Extensions.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Storage;


[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Storage")]
public class FileManagerController : BaseController
{
    private readonly IStorageManager _storageBiz;
    private readonly IUserIdentityService _identity;
    private readonly IJsonService _jsonService;

    public FileManagerController(IStorageManager storageBiz, IUserIdentityService identity, IJsonService jsonService)
    {
        _storageBiz = storageBiz;
        _identity = identity;
        _jsonService = jsonService;
    }


    [HttpPost("files/mine")]
    public async Task<IActionResult> Mine([FromBody] FileManagerDto model)
    {
        var op = await _storageBiz.Mine(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("files/delete")]
    public async Task<IActionResult> Delete([FromBody] FileManagerDeleteDto model)
    {
        var op = await _storageBiz.Delete(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("files/new-folder")]
    public async Task<IActionResult> NewFolder([FromBody] FileManagerNameDto model)
    {
        var op = await _storageBiz.NewFolder(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("files/rename")]
    public async Task<IActionResult> Rename([FromBody] FileManagerNameDto model)
    {
        var op = await _storageBiz.Rename(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("files/upload")]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> Upload()
    {
        var file = await Request.Form.Files.First().ToStorageItem();
        var model = _jsonService.Deserialize<FileManagerDto>(Request.Form["data"]!);
        var op = await _storageBiz.Upload(_identity.User!.UserId, file, model);
        return Json(op);
    }

    [HttpPost("files/shared-by-me")]
    public async Task<IActionResult> SharedByMe([FromBody] FileManagerDto model)
    {
        var op = await _storageBiz.SharedByMe(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("files/shared-by-others")]
    public async Task<IActionResult> SharedByOthers([FromBody] FileManagerDto model)
    {
        var op = await _storageBiz.SharedByOthers(_identity.User!.UserId, model);
        return Json(op);
    }
}