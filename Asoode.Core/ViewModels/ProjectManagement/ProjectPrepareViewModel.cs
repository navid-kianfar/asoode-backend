using System.Collections.Generic;
using Asoode.Core.ViewModels.Collaboration;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class ProjectPrepareViewModel
{
    public ProjectViewModel ViewModel { get; set; }
    public PendingInvitationViewModel[] PendingInvitations { get; set; }
    public MemberInfoViewModel[] AllInvited { get; set; }
    public List<InviteViewModel> InviteById { get; set; }
    public string[] EmailIdentities { get; set; }
    public MemberInfoViewModel User { get; set; }
    public dynamic Plan { get; set; }
}