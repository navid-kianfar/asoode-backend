using System;

namespace Asoode.Core.ViewModels.Reports;

public class DayReportViewModel
{
    public DateTime Date { get; set; }
    public int Done { get; set; }
    public int Blocked { get; set; }
}