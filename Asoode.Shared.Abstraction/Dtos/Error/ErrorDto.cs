using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.Error;

public record ErrorDto : BaseDto
{
    public string Description { get; set; }
    public string ErrorBody { get; set; }
    public int Index { get; set; }
}