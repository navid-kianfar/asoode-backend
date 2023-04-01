using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.Reports;

public class DashBoardEventViewModel
{
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public Guid RecordId { get; set; }
    public WorkPackageTaskState State { get; set; }
}