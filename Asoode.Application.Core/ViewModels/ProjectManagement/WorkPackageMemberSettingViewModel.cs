using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageMemberSettingViewModel : BaseViewModel
{
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public bool ShowTotalCards { get; set; }
    public ReceiveNotificationType ReceiveNotification { get; set; }
}