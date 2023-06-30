using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record ChangeAccessDto
{
    public AccessType Access { get; set; }
}