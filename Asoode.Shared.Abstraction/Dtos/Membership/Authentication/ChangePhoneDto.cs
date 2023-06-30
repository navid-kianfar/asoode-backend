using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record ChangePhoneDto
{
    [Required] public string Phone { get; set; }
}