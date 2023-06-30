using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Authentication;

public record RegisterRequestDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Marketer { get; set; }

    public string Password { get; set; }

    [Required] [MaxLength(200)] public string Username { get; set; }
}