using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class ChangePasswordViewModel
{
    [Required] [MinLength(6)] public string NewPassword { get; set; }
    [Required] [MinLength(6)] public string OldPassword { get; set; }
}