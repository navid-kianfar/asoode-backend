using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record InviteDto
{
    public string Id { get; set; }
    public AccessType Access { get; set; }
}