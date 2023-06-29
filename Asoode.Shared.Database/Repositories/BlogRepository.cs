using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Blog;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class BlogRepository : IBlogRepository
{
    private readonly ILoggerService _loggerService;
    private readonly WebsiteContext _context;

    public BlogRepository(ILoggerService loggerService, WebsiteContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }
    
    public async Task<OperationResult<BlogDto>> Blog(string culture)
    {
        try
        {
            var blog = await _context.Blogs.AsNoTracking()
                .SingleOrDefaultAsync(i => i.Culture == culture && i.Type == BlogType.Post);
                    
            if (blog == null) return OperationResult<BlogDto>.NotFound();
            return OperationResult<BlogDto>.Success(blog.ToDto());
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
            var query = _context.BlogPosts
                .Where(i => i.BlogId == blogId)
                .OrderByDescending(i => i.Priority)
                .ThenByDescending(i => i.CreatedAt);
            return await DbHelper.GetPaginatedData(query, tuple =>
            {
                var (items, startIndex) = tuple;
                return items.Select((i, index) =>
                {
                    var vm = i.ToDto();
                    vm.Index = startIndex + index + 1;
                    return vm;
                }).ToArray();
            }, model.Page, model.PageSize);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogRepository.Posts", e);
            return OperationResult<GridResult<PostDto>>.Failed();
        }
    }

    public async Task<OperationResult<PostDto>> Post(string postKey)
    {
        try
        {
            var post = await _context.BlogPosts
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Key == postKey);
            if (post == null) return OperationResult<PostDto>.NotFound();
            return OperationResult<PostDto>.Success(post.ToDto());
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
            var blog = await _context.Blogs.AsNoTracking()
                .SingleOrDefaultAsync(i => i.Culture == culture && i.Type == BlogType.Faq);
            if (blog == null) return OperationResult<BlogDto>.NotFound();
            return OperationResult<BlogDto>.Success(blog.ToDto());
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
            var posts = await _context.BlogPosts.Where(p => p.Culture == culture)
                .AsNoTracking()
                .OrderByDescending(i => i.CreatedAt)
                .ToArrayAsync();
            return OperationResult<PostDto[]>.Success(posts.Select(p => p.ToDto()).ToArray());
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
            var blog = await _context.Blogs.Where(b => b.Culture == culture)
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .ToArrayAsync();
                    
            return OperationResult<BlogDto[]>.Success(blog.Select(b => b.ToDto()).ToArray());
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogRepository.AllBlogs", e);
            return OperationResult<BlogDto[]>.Failed();
        }
    }
}