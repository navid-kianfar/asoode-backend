using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Blog;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Admin.Business.Implementation;

internal class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;

    public BlogService(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public Task<OperationResult<GridResult<BlogDto>>> BlogsList(Guid userId, GridFilterWithParams<GridQuery> model)
        => _blogRepository.BlogsList(userId, model);

    public Task<OperationResult<GridResult<PostDto>>> BlogPosts(Guid userId, Guid id,
        GridFilterWithParams<GridQuery> model)
        => _blogRepository.BlogPosts(userId, id, model);

    public Task<OperationResult<bool>> EditBlog(Guid userId, Guid id, BlogEditDto model)
        => _blogRepository.EditBlog(userId, id, model);

    public Task<OperationResult<bool>> CreateBlog(Guid userId, BlogEditDto model)
        => _blogRepository.CreateBlog(userId, model);

    public Task<OperationResult<bool>> CreatePost(Guid userId, Guid id, BlogPostEditDto model, StorageItemDto[] files)
        => _blogRepository.CreatePost(userId, id, model, files);

    public Task<OperationResult<bool>> EditPost(Guid userId, Guid id, BlogPostEditDto model, StorageItemDto[] files)
        => _blogRepository.EditPost(userId, id, model, files);

    public Task<OperationResult<bool>> DeletePost(Guid userId, Guid id)
        => _blogRepository.DeletePost(userId, id);
}