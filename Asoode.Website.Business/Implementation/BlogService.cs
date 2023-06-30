using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Blog;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;
using Asoode.Website.Abstraction.Contracts;

namespace Asoode.Website.Business.Implementation;

internal class BlogService : IBlogService
{
    private readonly IBlogRepository _repository;

    public BlogService(IBlogRepository repository)
    {
        _repository = repository;
    }

    public Task<OperationResult<BlogDto>> Blog(string culture)
        => _repository.Blog(culture);

    public Task<OperationResult<GridResult<PostDto>>> Posts(Guid blogId, GridFilter model)
        => _repository.Posts(blogId, model);

    public Task<OperationResult<PostDto>> Post(string blogId)
        => _repository.Post(blogId);

    public Task<OperationResult<BlogDto>> Faq(string culture)
        => _repository.Faq(culture);

    public Task<OperationResult<PostDto[]>> AllPosts(string culture)
        => _repository.AllPosts(culture);

    public Task<OperationResult<BlogDto[]>> AllBlogs(string culture)
        => _repository.AllBlogs(culture);
}