namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class ForgetPasswordResultViewModel
{
    public bool EmailFailed { get; set; }
    public bool EmailNotConfirmed { get; set; }
    public Guid Id { get; set; }
    public bool LockedOut { get; set; }
    public bool NotFound { get; set; }
    public bool PhoneNotConfirmed { get; set; }
    public bool SmsFailed { get; set; }
    public bool Wait { get; set; }
}