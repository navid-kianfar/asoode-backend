using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos;

public record TitleDto
{
    [Required] public string Title { get; set; }
}