using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Membership.Authentication;

public class ForgetPasswordViewModel
{
    [Required] public string Username { get; set; }
}