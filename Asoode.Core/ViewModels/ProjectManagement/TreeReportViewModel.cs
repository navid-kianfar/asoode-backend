using System;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class TreeReportViewModel
{
    public double TimeSpent { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Total { get; set; }
    public int Done { get; set; }
}