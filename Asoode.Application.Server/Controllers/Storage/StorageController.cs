using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Storage;

[Route("v2/storage")]
[ApiExplorerSettings(GroupName = "Storage")]
public class StorageController : BaseController
{
    private readonly IStorageService _storageManager;

    public StorageController(IStorageService storageManager)
    {
        _storageManager = storageManager;
    }

    [HttpGet]
    [Route("download/{*path}")]
    public async Task<IActionResult> DownloadPublic(string path)
    {
        var item = await _storageManager.Download(path);
        if (item == null) return NotFound();
        return File(item.Data.Stream!, item.Data.MimeType, item.Data.FileName);
    }
}