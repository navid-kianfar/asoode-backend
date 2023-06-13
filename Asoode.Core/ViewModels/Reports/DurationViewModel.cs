using System;
using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Reports;

public class DurationViewModel
{
    [Required] public DateTime Begin { get; set; }
    [Required] public DateTime End { get; set; }
}

public class DashboardDurationViewModel : DurationViewModel
{
    [Required] public DateTime MonthBegin { get; set; }
    [Required] public DateTime MonthEnd { get; set; }
}