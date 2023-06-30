using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record FileManagerDto
{
    [Required] public string Path { get; set; }
}