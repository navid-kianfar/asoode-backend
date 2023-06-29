using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.User;

public record UserResetPasswordDto
{
    [Required] [MinLength(6)] public string Password { get; set; }
}