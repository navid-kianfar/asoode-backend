using Asoode.Shared.Abstraction.Dtos;

namespace Asoode.Admin.Abstraction.Dtos;

public record ErrorDto : BaseDto
{
    public string Description { get; set; }
    public string ErrorBody { get; set; }
    public int Index { get; set; }
}