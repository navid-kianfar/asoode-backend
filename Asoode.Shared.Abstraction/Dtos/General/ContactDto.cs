using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record ContactDto
{
    [Required] [MaxLength(100)] public string FirstName { get; set; }
    [Required] [MaxLength(100)] public string LastName { get; set; }
    [Required] [MaxLength(200)] public string Email { get; set; }
    [Required] [MaxLength(1000)] public string Message { get; set; }
}