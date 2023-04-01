using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class ChangeEmailViewModel
{
    [EmailAddress] [Required] public string Email { get; set; }
}