using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record TitleDto
{
    [Required] public string Title { get; set; }
}