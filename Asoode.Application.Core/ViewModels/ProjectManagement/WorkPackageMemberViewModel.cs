using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageMemberViewModel : BaseViewModel
{
    public Guid RecordId { get; set; }
    public Guid PackageId { get; set; }
    public AccessType Access { get; set; }
    public bool BlockNotification { get; set; }
    public bool ShowStats { get; set; }
    public bool IsGroup { get; set; }
}