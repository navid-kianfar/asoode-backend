using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;
using Asoode.Website.Abstraction.Contracts;
using Asoode.Website.Abstraction.Dtos.Blog;

namespace Asoode.Website.Business.Implementation;

internal class BlogService : IBlogService
{
    private readonly IBlogRepository _repository;
    private readonly ILoggerService _loggerService;

    public BlogService(IBlogRepository repository, ILoggerService loggerService)
    {
        _repository = repository;
        _loggerService = loggerService;
    }
    
    public Task<OperationResult<BlogDto>> Blog(string culture)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<GridResult<PostDto>>> Posts(Guid blogId, GridFilter model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<PostDto>> Post(string blogId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<BlogDto>> Faq(string culture)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<PostDto[]>> AllPosts(string culture)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<BlogDto[]>> AllBlogs(string culture)
    {
        throw new NotImplementedException();
    }
}