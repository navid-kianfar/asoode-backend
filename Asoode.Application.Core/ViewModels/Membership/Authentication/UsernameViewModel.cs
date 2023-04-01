using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class UsernameViewModel
{
    [Required] public string Username { get; set; }
}