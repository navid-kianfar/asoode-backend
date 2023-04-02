using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.General
{
    [Route("v2")]
    [ApiExplorerSettings(GroupName = "General")]
    public class GeneralController : BaseController
    {
        private readonly IServiceProvider _serviceProvider;

        public GeneralController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost("captcha")]
        public IActionResult Captcha()
        {
            var result = _serviceProvider.GetService<ICaptchaBiz>().Generate();
            return Json(result);
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
}