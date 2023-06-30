using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record RecoverPasswordDto
{
    [Required] public string Code { get; set; }
    [Required] public Guid Id { get; set; }
    [Required] public string Password { get; set; }
}