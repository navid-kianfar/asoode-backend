using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageTaskVoteViewModel : BaseViewModel
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public bool Vote { get; set; }
}