using Asoode.Admin.Abstraction.Dtos;
using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[Route(EndpointConstants.Prefix)]
public class BlogController : BaseController
{
    private readonly IBlogBiz _blogBiz;
    private readonly IUserIdentityService _identity;
    private readonly IJsonService _jsonService;

    public BlogController(
        IBlogBiz blogBiz,
        IJsonService jsonService,
        IUserIdentityService identity)
    {
        _blogBiz = blogBiz;
        _jsonService = jsonService;
        _identity = identity;
    }


    [HttpPost("blog/list")]
    public async Task<IActionResult> List([FromBody] GridFilterWithParams<GridQuery> model)
    {
        var op = await _blogBiz.AdminBlogsList(_identity.User!.UserId, model);
        return Json(op);
    }


    [HttpPost("blog/edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] BlogEditDto model)
    {
        var op = await _blogBiz.AdminEditBlog(_identity.User!.UserId, id, model);
        return Json(op);
    }


    [HttpPost("blog/create")]
    public async Task<IActionResult> Create([FromBody] BlogEditDto model)
    {
        var op = await _blogBiz.AdminCreateBlog(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("blog/posts/{id:guid}")]
    public async Task<IActionResult> Posts(Guid id, [FromBody] GridFilterWithParams<GridQuery> model)
    {
        var op = await _blogBiz.AdminBlogPosts(_identity.User!.UserId, id, model);
        return Json(op);
    }


    [HttpPost("blog/{id:guid}/post/create")]
    public async Task<IActionResult> CreatePost(Guid id)
    {
        var files = await Request.Form.Files.ToStorageItems();
        var model = _jsonService.Deserialize<BlogPostEditDto>(Request.Form["data"]!);
        var op = await _blogBiz.AdminCreatePost(_identity.User!.UserId, id, model, files);
        return Json(op);
    }

    [HttpPost("blog/post/edit/{id:guid}")]
    public async Task<IActionResult> EditPost(Guid id)
    {
        var files = await Request.Form.Files.ToStorageItems();
        var model = _jsonService.Deserialize<BlogPostEditDto>(Request.Form["data"]!);
        var op = await _blogBiz.AdminEditPost(_identity.User!.UserId, id, model, files);
        return Json(op);
    }

    [HttpPost("blog/post/delete/{id:guid}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var op = await _blogBiz.AdminDeletePost(_identity.User!.UserId, id);
        return Json(op);
    }
}