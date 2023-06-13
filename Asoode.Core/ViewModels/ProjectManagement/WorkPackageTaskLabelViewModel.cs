using System;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class WorkPackageTaskLabelViewModel : BaseViewModel
{
    public Guid TaskId { get; set; }
    public Guid LabelId { get; set; }
    public Guid PackageId { get; set; }
}