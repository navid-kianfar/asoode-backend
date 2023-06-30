using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Blog;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface IBlogRepository
{
    Task<OperationResult<BlogDto>> Blog(string culture);
    Task<OperationResult<GridResult<PostDto>>> Posts(Guid blogId, GridFilter model);
    Task<OperationResult<PostDto>> Post(string blogId);
    Task<OperationResult<BlogDto>> Faq(string culture);
    Task<OperationResult<BlogDto[]>> AllBlogs(string culture);
    Task<OperationResult<PostDto[]>> AllPosts(string culture);
    Task<OperationResult<bool>> DeletePost(Guid userId, Guid id);
    Task<OperationResult<bool>> EditPost(Guid userId, Guid id, BlogPostEditDto model);
    Task<OperationResult<bool>> CreatePost(Guid userId, Guid id, BlogPostEditDto model);
    Task<OperationResult<bool>> CreateBlog(Guid userId, BlogEditDto model);
    Task<OperationResult<bool>> EditBlog(Guid userId, Guid id, BlogEditDto model);
    Task<OperationResult<GridResult<PostDto>>> BlogPosts(Guid userId, Guid id, GridFilterWithParams<GridQuery> model);
    Task<OperationResult<GridResult<BlogDto>>> BlogsList(Guid userId, GridFilterWithParams<GridQuery> model);
    Task<OperationResult<BlogDto>> GetBlog(Guid id);
    Task<OperationResult<PostDto>> GetPost(Guid id);
}