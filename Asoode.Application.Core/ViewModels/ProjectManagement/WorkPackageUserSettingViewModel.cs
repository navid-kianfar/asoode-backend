using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageUserSettingViewModel
{
    public ReceiveNotificationType? NotificationType { get; set; }
    public bool? ShowTotal { get; set; }
}