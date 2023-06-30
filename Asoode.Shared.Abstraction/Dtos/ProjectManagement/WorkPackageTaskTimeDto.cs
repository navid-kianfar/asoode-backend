using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageTaskTimeDto : BaseDto
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public DateTime Begin { get; set; }
    public DateTime? End { get; set; }
    public bool Manual { get; set; }
}