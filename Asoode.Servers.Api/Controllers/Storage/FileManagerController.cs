using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Storage;
using Asoode.Application.Core.ViewModels.Storage;
using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Extensions;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.Storage
{
    [JwtAuthorize]
    [Route("v2/files")]
    [ApiExplorerSettings(GroupName = "Storage")]
    public class FileManagerController : BaseController
    {
        private readonly IStorageBiz _storageBiz;
        private readonly IServiceProvider _serviceProvider;

        public FileManagerController(IStorageBiz storageBiz, IServiceProvider serviceProvider)
        {
            _storageBiz = storageBiz;
            _serviceProvider = serviceProvider;
        }
        
        
        [HttpPost("mine")]
        [ValidateModel]
        public async Task<IActionResult> Mine([FromBody] FileManagerViewModel model)
        {
            var op = await _storageBiz.Mine(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("delete")]
        [ValidateModel]
        public async Task<IActionResult> Delete([FromBody] FileManagerDeleteViewModel model)
        {
            var op = await _storageBiz.Delete(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("new-folder")]
        [ValidateModel]
        public async Task<IActionResult> NewFolder([FromBody] FileManagerNameViewModel model)
        {
            var op = await _storageBiz.NewFolder(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("rename")]
        [ValidateModel]
        public async Task<IActionResult> Rename([FromBody] FileManagerNameViewModel model)
        {
            var op = await _storageBiz.Rename(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> Upload()
        {
            var jsonBiz = _serviceProvider.GetService<IJsonService>();
            var file = await Request.Form.Files?.FirstOrDefault()?.ToViewModel();
            var model = jsonBiz.Deserialize<FileManagerViewModel>(Request.Form["data"]);
            var op = await _storageBiz.Upload(Identity.UserId, file, model);
            return Json(op);
        }

        [HttpPost("shared-by-me")]
        [ValidateModel]
        public async Task<IActionResult> SharedByMe([FromBody] FileManagerViewModel model)
        {
            var op = await _storageBiz.SharedByMe(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("shared-by-others")]
        [ValidateModel]
        public async Task<IActionResult> SharedByOthers([FromBody] FileManagerViewModel model)
        {
            var op = await _storageBiz.SharedByOthers(Identity.UserId, model);
            return Json(op);
        }
    }
}