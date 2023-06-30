namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record LoginResultDto
{
    public bool EmailNotConfirmed { get; set; }
    public bool InvalidPassword { get; set; }
    public bool LockedOut { get; set; }
    public bool NotFound { get; set; }
    public bool PhoneNotConfirmed { get; set; }
    public bool SmsFailed { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public Guid Id { get; set; }
    public bool EmailFailed { get; set; }
    public DateTime? LockedUntil { get; set; }
}