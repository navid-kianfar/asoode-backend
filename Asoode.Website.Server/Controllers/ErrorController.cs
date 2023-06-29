using Microsoft.AspNetCore.Mvc;

namespace Asoode.Website.Server.Controllers
{
    [Localize]
    [Route("error")]
    public class ErrorController : Controller
    {
        [HttpGet("400")]
        public IActionResult E404()
        {
            return View("NotFound");
        }
        
        [HttpGet("500")]
        public IActionResult E500()
        {
            return View("Server");
        }
    }
}