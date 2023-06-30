using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record ConfirmVerificationDto
{
    [Required]
    [MinLength(6)]
    [MaxLength(6)]
    public string Code { get; set; }

    [Required] public Guid Id { get; set; }
}