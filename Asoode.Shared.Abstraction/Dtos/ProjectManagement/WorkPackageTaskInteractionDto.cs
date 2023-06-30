using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageTaskInteractionDto : BaseDto
{
    public Guid TaskId { get; set; }
    public Guid PackageId { get; set; }
    public DateTime? LastView { get; set; }
    public bool? Watching { get; set; }
    public bool? Vote { get; set; }
}