using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Reports;

public class DashboardDurationViewModel : DurationViewModel
{
    [Required] public DateTime MonthBegin { get; set; }
    [Required] public DateTime MonthEnd { get; set; }
}