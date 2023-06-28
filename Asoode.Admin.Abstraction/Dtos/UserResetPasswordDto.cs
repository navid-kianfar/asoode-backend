using System.ComponentModel.DataAnnotations;

namespace Asoode.Admin.Abstraction.Dtos;

public record UserResetPasswordDto
{
    [Required] [MinLength(6)] public string Password { get; set; }
}