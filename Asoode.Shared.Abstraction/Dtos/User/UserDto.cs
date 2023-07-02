using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.User;

public record UserDto
{
    public Guid Id { get; set; }
    public UserType Type { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public bool Blocked { get; set; }

    public DateTime CreatedAt { get; set; }
    // public DateTime? DeletedAt { get; set; }
    // public DateTime? BlockedAt { get; set; }
    // public string Hash { get; set; } = string.Empty;
    // public string Salt { get; set; } = string.Empty;
    // public int FailedAttempt { get; set; }
    // public string? Name { get; set; }
    // public string? Surname { get; set; }
    // public string? Phone { get; set; }
    // public string? Avatar { get; set; }
}