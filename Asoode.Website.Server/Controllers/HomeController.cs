using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Website.Abstraction.Contracts;
using Asoode.Website.Abstraction.Dtos.Blog;
using Asoode.Website.Server.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Website.Server.Controllers
{
    [Localize]
    public class HomeController : Controller
    {
        private readonly IServiceProvider _serviceProvider;

        public HomeController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IActionResult SiteMap()
        {
            Response.ContentType = "text/xml";
            return View();
        }
        public IActionResult Rss()
        {
            Response.ContentType = "text/xml";
            return View();
        }
        public IActionResult Index()
        {
            if (Request.QueryString.HasValue)
            {
                string marketer = Request.Query["marketer"].ToString();
                if (!string.IsNullOrEmpty(marketer))
                {
                    Response.Cookies.Delete("MARKETER");
                    Response.Cookies.Append("MARKETER", marketer, new CookieOptions
                    {
                        HttpOnly = false,
                        Path = "/",
                        IsEssential = true,
                        Domain = EnvironmentHelper.Get("APP_DOMAIN")
                    });
                }
            }
            return View();
        }
        public IActionResult Why()
        {
            return View();
        }
        // public async Task<IActionResult> Plans()
        // {
        //     var op = await _serviceProvider.GetService<IPlanBiz>().List();
        //     return View(op.Data);
        // }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public async Task<IActionResult> Faq(int page = 1)
        {
            var blogBiz = _serviceProvider.GetService<IBlogService>()!;
            var culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            var blog = await blogBiz.Faq(culture);
            var posts = await blogBiz.Posts(blog.Data!.Id, new GridFilter
            {
                Page = page,
                PageSize = 20
            });
            return View(new BlogResultDto
            {
                Blog = blog.Data,
                Posts = posts.Data
            });
        }
        public async Task<IActionResult> Blog(int page = 1)
        {
            var blogBiz = _serviceProvider.GetService<IBlogService>()!;
            var culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            var blog = await blogBiz.Blog(culture);
            var posts = await blogBiz.Posts(blog.Data!.Id, new GridFilter
            {
                Page = page,
                PageSize = 5
            });
            return View(new BlogResultDto
            {
                Blog = blog.Data,
                Posts = posts.Data
            });
        }
        
        public async Task<IActionResult> Post(string key, string title)
        {
            var blogBiz = _serviceProvider.GetService<IBlogService>()!;
            var postOp = await blogBiz.Post(key);
            if (postOp.Status != OperationResultStatus.Success) return Redirect("/");
            return View(postOp.Data!);
        }
    }
}