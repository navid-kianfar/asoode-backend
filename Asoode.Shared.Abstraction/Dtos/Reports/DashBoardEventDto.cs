using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record DashBoardEventDto
{
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public Guid RecordId { get; set; }
    public WorkPackageTaskState State { get; set; }
}