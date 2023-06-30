using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.User;

public record UserClaimDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserType Type { get; set; }
}