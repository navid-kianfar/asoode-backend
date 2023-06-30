using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record DurationDto
{
    [Required] public DateTime Begin { get; set; }
    [Required] public DateTime End { get; set; }
}