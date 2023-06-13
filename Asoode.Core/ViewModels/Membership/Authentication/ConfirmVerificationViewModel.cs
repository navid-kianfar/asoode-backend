using System;
using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Membership.Authentication;

public class ConfirmVerificationViewModel
{
    [Required]
    [MinLength(6)]
    [MaxLength(6)]
    public string Code { get; set; }

    [Required] public Guid Id { get; set; }
}