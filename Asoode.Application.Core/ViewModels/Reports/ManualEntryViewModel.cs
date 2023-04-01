using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Reports;

public class ManualEntryViewModel : DurationViewModel
{
    [Required] public Guid UserId { get; set; }
}