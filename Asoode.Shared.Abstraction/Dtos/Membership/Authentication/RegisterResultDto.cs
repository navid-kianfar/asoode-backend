namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record RegisterResultDto
{
    public bool Duplicate { get; set; }
    public bool EmailFailed { get; set; }
    public bool EmailNotConfirmed { get; set; }
    public bool PhoneNotConfirmed { get; set; }
    public bool SmsFailed { get; set; }
    public Guid Id { get; set; }
}