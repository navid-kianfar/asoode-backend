using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record ChangePasswordDto
{
    [Required] [MinLength(6)] public string NewPassword { get; set; }
    [Required] [MinLength(6)] public string OldPassword { get; set; }
}