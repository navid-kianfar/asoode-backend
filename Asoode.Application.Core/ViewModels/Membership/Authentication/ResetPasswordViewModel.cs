using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class ResetPasswordViewModel
{
    [Required] [MinLength(6)] public string NewPassword { get; set; }
}