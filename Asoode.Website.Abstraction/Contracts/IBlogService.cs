using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Blog;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Website.Abstraction.Contracts;

public interface IBlogService
{
    Task<OperationResult<BlogDto>> Blog(string culture);
    Task<OperationResult<GridResult<PostDto>>> Posts(Guid blogId, GridFilter model);
    Task<OperationResult<PostDto>> Post(string blogId);
    Task<OperationResult<BlogDto>> Faq(string culture);
    Task<OperationResult<PostDto[]>> AllPosts(string culture);
    Task<OperationResult<BlogDto[]>> AllBlogs(string culture);
}