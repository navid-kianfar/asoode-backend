using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record ResetPasswordDto
{
    [Required] [MinLength(6)] public string NewPassword { get; set; }
}