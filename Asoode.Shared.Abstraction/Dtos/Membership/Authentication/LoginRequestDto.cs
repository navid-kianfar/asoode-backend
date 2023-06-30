using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record LoginRequestDto
{
    [Required] [MinLength(6)] public string Password { get; set; }
    [Required] public string Username { get; set; }
}