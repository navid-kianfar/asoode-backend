using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Membership.Authentication;

public class UsernameViewModel
{
    [Required] public string Username { get; set; }
}