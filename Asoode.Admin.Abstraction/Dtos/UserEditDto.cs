using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Admin.Abstraction.Dtos;

public record UserEditDto
{
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public UserType Type { get; set; }
}