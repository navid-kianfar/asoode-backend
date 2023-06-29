using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Blog;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class BlogRepository : IBlogRepository
{
    private readonly ILoggerService _loggerService;

    public BlogRepository(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }
    
    public async Task<OperationResult<BlogDto>> Blog(string culture)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogRepository.Blog", e);
            return OperationResult<BlogDto>.Failed();
        }
    }

    public async Task<OperationResult<GridResult<PostDto>>> Posts(Guid blogId, GridFilter model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogRepository.Posts", e);
            return OperationResult<GridResult<PostDto>>.Failed();
        }
    }

    public async Task<OperationResult<PostDto>> Post(string blogId)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogRepository.Post", e);
            return OperationResult<PostDto>.Failed();
        }
    }

    public async Task<OperationResult<BlogDto>> Faq(string culture)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogRepository.Faq", e);
            return OperationResult<BlogDto>.Failed();
        }
    }

    public async Task<OperationResult<PostDto[]>> AllPosts(string culture)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogRepository.AllPosts", e);
            return OperationResult<PostDto[]>.Failed();
        }
    }

    public async Task<OperationResult<BlogDto[]>> AllBlogs(string culture)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogRepository.AllBlogs", e);
            return OperationResult<BlogDto[]>.Failed();
        }
    }
}