using System;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.Reports;

public class DashBoardEventViewModel
{
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public Guid RecordId { get; set; }
    public WorkPackageTaskState State { get; set; }
}