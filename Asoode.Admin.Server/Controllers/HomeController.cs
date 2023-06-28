using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}