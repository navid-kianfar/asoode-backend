using Microsoft.AspNetCore.Mvc;

namespace Asoode.Website.Server.Controllers
{
    [Localize]
    public class HomeController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public HomeController(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
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
                        Domain = _configuration["Setting:Domain"]
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
            var blogBiz = _serviceProvider.GetService<IBlogBiz>();
            var culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            var blog = await blogBiz.Faq(culture);
            var posts = await blogBiz.Posts(blog.Data.Id, new GridFilter
            {
                Page = page,
                PageSize = 20
            });
            return View(new BlogResultViewModel
            {
                Blog = blog.Data,
                Posts = posts.Data
            });
        }
        public async Task<IActionResult> Blog(int page = 1)
        {
            var blogBiz = _serviceProvider.GetService<IBlogBiz>();
            var culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            var blog = await blogBiz.Blog(culture);
            var posts = await blogBiz.Posts(blog.Data.Id, new GridFilter
            {
                Page = page,
                PageSize = 5
            });
            return View(new BlogResultViewModel
            {
                Blog = blog.Data,
                Posts = posts.Data
            });
        }
        
        public async Task<IActionResult> Post(string key, string title)
        {
            var blogBiz = _serviceProvider.GetService<IBlogBiz>();
            var post = await blogBiz.Post(key);
            if (post == null) return Redirect("/");
            return View(post.Data);
        }
    }
}