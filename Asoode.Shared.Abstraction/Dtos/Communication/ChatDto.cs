using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Communication;

public record ChatDto
{
    [Required] public string Message { get; set; }
}