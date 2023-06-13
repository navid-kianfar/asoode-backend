using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class WorkPackageMemberViewModel : BaseViewModel
{
    public Guid RecordId { get; set; }
    public Guid PackageId { get; set; }
    public AccessType Access { get; set; }
    public bool BlockNotification { get; set; }
    public bool ShowStats { get; set; }
    public bool IsGroup { get; set; }
}