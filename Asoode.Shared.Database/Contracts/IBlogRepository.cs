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
}