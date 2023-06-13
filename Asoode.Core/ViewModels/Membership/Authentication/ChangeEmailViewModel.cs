using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Membership.Authentication;

public class ChangeEmailViewModel
{
    [EmailAddress] [Required] public string Email { get; set; }
}