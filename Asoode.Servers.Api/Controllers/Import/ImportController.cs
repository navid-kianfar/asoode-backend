using Asoode.Application.Core.Contracts.Import;
using Asoode.Application.Core.ViewModels.Import.Trello;
using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Extensions;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Asoode.Servers.Api.Controllers.Import
{
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
            var file = await Request.Form.Files?.FirstOrDefault()?.ToViewModel();
            var data = JsonConvert.DeserializeObject<TrelloMapedDataViewModel>(Request.Form["data"]!)!;
            var op = await _serviceProvider
                .GetService<ITrelloBiz>()!
                .Import(file, data, Identity.UserId);
            return Json(op);
        }
    }
}