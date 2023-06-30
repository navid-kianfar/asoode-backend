using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record FileManagerDeleteDto
{
    [Required] public string[] Paths { get; set; }
}