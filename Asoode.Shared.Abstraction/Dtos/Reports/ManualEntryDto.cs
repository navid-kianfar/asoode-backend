using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record ManualEntryDto : DurationDto
{
    [Required] public Guid UserId { get; set; }
}