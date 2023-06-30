using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record SimpleDto
{
    [Required] public string Title { get; set; }
    public string Description { get; set; }
}