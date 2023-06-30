using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record FileManagerNameDto : FileManagerDto
{
    [Required] public string Name { get; set; }
}