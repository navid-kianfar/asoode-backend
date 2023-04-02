using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class ProjectViewModel : BaseViewModel
{
    public ProjectViewModel()
    {
        Seasons = Array.Empty<ProjectSeasonViewModel>();
        SubProjects = Array.Empty<SubProjectViewModel>();
        WorkPackages = Array.Empty<WorkPackageViewModel>();
        Members = Array.Empty<ProjectMemberViewModel>();
        Pending = Array.Empty<PendingInvitationViewModel>();
    }

    public int AttachmentSize { get; set; }

    public Guid UserId { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Complex { get; set; }
    public ProjectTemplate Template { get; set; }
    public bool Premium { get; set; }

    public ProjectMemberViewModel[] Members { get; set; }
    public ProjectSeasonViewModel[] Seasons { get; set; }
    public SubProjectViewModel[] SubProjects { get; set; }
    public WorkPackageViewModel[] WorkPackages { get; set; }
    public PendingInvitationViewModel[] Pending { get; set; }
}