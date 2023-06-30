using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record ChangeEmailDto
{
    [EmailAddress] [Required] public string Email { get; set; }
}