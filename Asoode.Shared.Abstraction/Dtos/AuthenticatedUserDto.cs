using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos;

public record AuthenticatedUserDto
{
    public Guid UserId { get; set; }
    public Guid TokenId { get; set; }
    public UserType UserType { get; set; }
    public string Username { get; set; }
}