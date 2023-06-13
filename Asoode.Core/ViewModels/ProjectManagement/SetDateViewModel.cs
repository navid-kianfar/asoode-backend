using System;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class SetDateViewModel
{
    public DateTime? DueAt { get; set; }
    public DateTime? BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
}