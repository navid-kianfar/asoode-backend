using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Abstraction.Contracts;

public interface IBlogService
{
    Task<OperationResult<GridResult<BlogDto>>> BlogsList(Guid userId, GridFilterWithParams<GridQuery> model);

    Task<OperationResult<GridResult<PostDto>>> BlogPosts(Guid userId, Guid id,
        GridFilterWithParams<GridQuery> model);

    Task<OperationResult<bool>> EditBlog(Guid userId, Guid id, BlogEditDto model);
    Task<OperationResult<bool>> CreateBlog(Guid userId, BlogEditDto model);

    Task<OperationResult<bool>> CreatePost(Guid userId, Guid id, BlogPostEditDto model,
        StorageItemDto[] files);

    Task<OperationResult<bool>> EditPost(Guid userId, Guid id, BlogPostEditDto model,
        StorageItemDto[] files);

    Task<OperationResult<bool>> DeletePost(Guid userId, Guid id);
}