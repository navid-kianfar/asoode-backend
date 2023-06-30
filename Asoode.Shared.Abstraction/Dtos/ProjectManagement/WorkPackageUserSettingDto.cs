using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageUserSettingDto
{
    public ReceiveNotificationType? NotificationType { get; set; }
    public bool? ShowTotal { get; set; }
}