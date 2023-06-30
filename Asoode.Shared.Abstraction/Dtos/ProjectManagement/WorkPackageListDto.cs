using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageListDto : BaseDto
{
    public Guid PackageId { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public bool DarkColor { get; set; }
    public int Order { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public bool Restricted { get; set; }
    public WorkPackageTaskState? Kanban { get; set; }
}