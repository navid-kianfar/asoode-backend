using Asoode.Application.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Fixtures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Storage;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Storage")]
public class StorageController : BaseController
{
    private readonly IStorageManager _storageManager;

    public StorageController(IStorageManager storageManager)
    {
        _storageManager = storageManager;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("storage/download/{*path}")]
    public async Task<IActionResult> DownloadPublic(string path)
    {
        var item = await _storageManager.DownloadPublic(path);
        if (item == null) return NotFound();
        return File(item.Stream!, item.MimeType, item.FileName);
    }
}