using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.General.Search;

public record SearchRequestDto
{
    [Required] public string Search { get; set; }
}