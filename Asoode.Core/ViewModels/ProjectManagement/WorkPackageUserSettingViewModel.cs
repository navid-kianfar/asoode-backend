using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class WorkPackageUserSettingViewModel
{
    public ReceiveNotificationType? NotificationType { get; set; }
    public bool? ShowTotal { get; set; }
}