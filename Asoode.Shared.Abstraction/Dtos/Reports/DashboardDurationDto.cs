using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record DashboardDurationDto : DurationDto
{
    [Required] public DateTime MonthBegin { get; set; }
    [Required] public DateTime MonthEnd { get; set; }
}