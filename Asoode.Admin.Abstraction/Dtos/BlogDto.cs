using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Admin.Abstraction.Dtos;

public record BlogDto : BaseDto
{
    public int Index { get; set; }

    public BlogType Type { get; set; }
    public string Culture { get; set; }
    public string Keywords { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
}