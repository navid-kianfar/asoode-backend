using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Reports;

public class DurationViewModel
{
    [Required] public DateTime Begin { get; set; }
    [Required] public DateTime End { get; set; }
}