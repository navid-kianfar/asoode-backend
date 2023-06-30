using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageMemberDto : BaseDto
{
    public Guid RecordId { get; set; }
    public Guid PackageId { get; set; }
    public AccessType Access { get; set; }
    public bool BlockNotification { get; set; }
    public bool ShowStats { get; set; }
    public bool IsGroup { get; set; }
}