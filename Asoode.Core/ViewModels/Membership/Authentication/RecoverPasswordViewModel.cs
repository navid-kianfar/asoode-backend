using System;
using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Membership.Authentication;

public class RecoverPasswordViewModel
{
    [Required] public string Code { get; set; }
    [Required] public Guid Id { get; set; }
    [Required] public string Password { get; set; }
}