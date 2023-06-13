using System.Linq;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Extensions;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Storage;

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