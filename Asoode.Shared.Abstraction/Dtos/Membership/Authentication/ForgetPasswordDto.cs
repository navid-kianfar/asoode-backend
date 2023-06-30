using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record ForgetPasswordDto
{
    [Required] public string Username { get; set; }
}