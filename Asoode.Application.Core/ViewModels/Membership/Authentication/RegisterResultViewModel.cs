namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class RegisterResultViewModel
{
    public bool Duplicate { get; set; }
    public bool EmailFailed { get; set; }
    public bool EmailNotConfirmed { get; set; }
    public bool PhoneNotConfirmed { get; set; }
    public bool SmsFailed { get; set; }
    public Guid Id { get; set; }
}