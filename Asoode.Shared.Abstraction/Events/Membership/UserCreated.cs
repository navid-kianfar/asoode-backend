namespace Asoode.Shared.Abstraction.Events.Membership;

public record UserCreated(
    Guid Id,
    string Username,
    string Email,
    string LastName,
    string FirstName,
    DateTime CreatedAt,
    string? Marketer
);