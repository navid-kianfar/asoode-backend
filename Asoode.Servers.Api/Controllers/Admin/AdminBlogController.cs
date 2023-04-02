using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Admin;
using Asoode.Core.Contracts.General;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Backend.Controllers.Admin
{
    [JwtAuthorize(UserType.Admin)]
    [Route("v2/admin/blog")]
    public class AdminBlogController : BaseController
    {
        private readonly IBlogBiz _blogBiz;
        private readonly IServiceProvider _serviceProvider;

        public AdminBlogController(IBlogBiz blogBiz, IServiceProvider serviceProvider)
        {
            _blogBiz = blogBiz;
            _serviceProvider = serviceProvider;
        }
        
        [ValidateModel]
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] GridFilterWithParams<GridQuery> model)
        {
            var op = await _blogBiz.AdminBlogsList(Identity.UserId, model);
            return Json(op);
        }
        
        [ValidateModel]
        [HttpPost("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] BlogEditViewModel model)
        {
            var op = await _blogBiz.AdminEditBlog(Identity.UserId, id, model);
            return Json(op);
        }
        
        [ValidateModel]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BlogEditViewModel model)
        {
            var op = await _blogBiz.AdminCreateBlog(Identity.UserId, model);
            return Json(op);
        }
        
        [HttpPost("posts/{id:guid}")]
        public async Task<IActionResult> Posts(Guid id, [FromBody] GridFilterWithParams<GridQuery> model)
        {
            var op = await _blogBiz.AdminBlogPosts(Identity.UserId, id, model);
            return Json(op);
        }
        
        [HttpPost("{id:guid}/post/create")]
        [ValidateModel]
        public async Task<IActionResult> CreatePost(Guid id)
        {
            var jsonBiz = _serviceProvider.GetService<IJsonBiz>();
            var files = Request.Form.Files?.ToArray();
            var model = jsonBiz.Deserialize<BlogPostEditViewModel>(Request.Form["data"]);
            var op = await _blogBiz.AdminCreatePost(Identity.UserId, id, model, files);
            return Json(op);
        }
        
        [HttpPost("post/edit/{id:guid}")]
        public async Task<IActionResult> EditPost(Guid id)
        {
            var jsonBiz = _serviceProvider.GetService<IJsonBiz>();
            var files = Request.Form.Files?.ToArray();
            var model = jsonBiz.Deserialize<BlogPostEditViewModel>(Request.Form["data"]);
            var op = await _blogBiz.AdminEditPost(Identity.UserId, id, model, files);
            return Json(op);
        }
        
        [HttpPost("post/delete/{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var op = await _blogBiz.AdminDeletePost(Identity.UserId, id);
            return Json(op);
        }
    }
}