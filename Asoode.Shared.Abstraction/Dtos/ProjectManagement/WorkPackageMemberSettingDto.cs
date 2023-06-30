using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageMemberSettingDto : BaseDto
{
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public bool ShowTotalCards { get; set; }
    public ReceiveNotificationType ReceiveNotification { get; set; }
}