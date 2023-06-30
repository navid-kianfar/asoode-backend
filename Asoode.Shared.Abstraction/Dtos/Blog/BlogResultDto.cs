using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.Blog;

public record BlogResultDto
{
    public BlogDto Blog { get; set; }
    public GridResult<PostDto> Posts { get; set; }
}