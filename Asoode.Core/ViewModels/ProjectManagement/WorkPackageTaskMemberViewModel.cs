using System;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class WorkPackageTaskMemberViewModel : BaseViewModel
{
    public Guid TaskId { get; set; }
    public Guid PackageId { get; set; }
    public bool IsGroup { get; set; }
    public Guid RecordId { get; set; }
}