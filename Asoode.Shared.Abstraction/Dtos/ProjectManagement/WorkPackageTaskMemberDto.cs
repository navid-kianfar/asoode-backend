using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageTaskMemberDto : BaseDto
{
    public Guid TaskId { get; set; }
    public Guid PackageId { get; set; }
    public bool IsGroup { get; set; }
    public Guid RecordId { get; set; }
}