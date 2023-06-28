using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class BlogService : IBlogService
{
    public Task<OperationResult<GridResult<BlogDto>>> BlogsList(Guid userId, GridFilterWithParams<GridQuery> model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<GridResult<PostDto>>> BlogPosts(Guid userId, Guid id,
        GridFilterWithParams<GridQuery> model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditBlog(Guid userId, Guid id, BlogEditDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CreateBlog(Guid userId, BlogEditDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CreatePost(Guid userId, Guid id, BlogPostEditDto model, StorageItemDto[] files)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditPost(Guid userId, Guid id, BlogPostEditDto model, StorageItemDto[] files)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> DeletePost(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}