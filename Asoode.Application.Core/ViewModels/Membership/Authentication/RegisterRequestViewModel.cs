using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class RegisterRequestViewModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Marketer { get; set; }

    public string Password { get; set; }

    [Required] [MaxLength(200)] public string Username { get; set; }
}