using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Admin;

public class UserResetPasswordViewModel
{
    [Required] [MinLength(6)] public string Password { get; set; }
}