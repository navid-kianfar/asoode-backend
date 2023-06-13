using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Membership.Authentication;

public class LoginRequestViewModel
{
    [Required] [MinLength(6)] public string Password { get; set; }
    [Required] public string Username { get; set; }
}