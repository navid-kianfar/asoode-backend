using System;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class WorkPackageTaskInteractionViewModel : BaseViewModel
{
    public Guid TaskId { get; set; }
    public Guid PackageId { get; set; }
    public DateTime? LastView { get; set; }
    public bool? Watching { get; set; }
    public bool? Vote { get; set; }
}