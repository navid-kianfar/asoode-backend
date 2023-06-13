using System;
using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Reports;

public class ManualEntryViewModel : DurationViewModel
{
    [Required] public Guid UserId { get; set; }
}