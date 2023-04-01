using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.Collaboration;
using Asoode.Application.Core.ViewModels.Communication;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.ProjectManagement;

namespace Asoode.Application.Core.ViewModels.Logging;

public class ActivityLogViewModel
{
    public ActivityType Type { get; set; }
    public Guid UserId { get; set; }
    public GroupViewModel Group { get; set; }
    public ProjectViewModel Project { get; set; }
    public WorkPackageViewModel WorkPackage { get; set; }
    public MemberInfoViewModel User { get; set; }
    public PendingInvitationViewModel[] Pendings { get; set; }
    public GroupMemberViewModel[] GroupMembers { get; set; }
    public ProjectMemberViewModel[] ProjectMembers { get; set; }
    public WorkPackageMemberViewModel[] WorkPackageMembers { get; set; }
    public Guid[] UserIds { get; set; }

    public PendingInvitationViewModel Pending { get; set; }
    public GroupMemberViewModel GroupMember { get; set; }
    public ProjectMemberViewModel ProjectMember { get; set; }
    public WorkPackageMemberViewModel WorkPackageMember { get; set; }
    public ConversationViewModel Conversation { get; set; }
    public SubProjectViewModel SubProject { get; set; }
    public ProjectSeasonViewModel Season { get; set; }
    public WorkPackageMemberSettingViewModel PackageUserSetting { get; set; }
    public WorkPackageLabelViewModel PackageLabel { get; set; }
    public WorkPackageObjectiveViewModel Objective { get; set; }
    public WorkPackageListViewModel PackageList { get; set; }
    public WorkPackageTaskViewModel PackageTask { get; set; }
    public WorkPackageTaskVoteViewModel PackageTaskVote { get; set; }
    public WorkPackageTaskTimeViewModel PackageTaskTime { get; set; }
    public WorkPackageTaskInteractionViewModel PackageTaskInteraction { get; set; }
    public WorkPackageTaskCommentViewModel PackageTaskComment { get; set; }
    public WorkPackageTaskLabelViewModel PackageTaskLabel { get; set; }
    public WorkPackageTaskAttachmentViewModel PackageTaskAttachment { get; set; }
    public WorkPackageTaskMemberViewModel WorkPackageTaskMember { get; set; }
    public DeviceViewModel Device { get; set; }
    public WorkPackageViewModel WorkPackage2 { get; set; }
    public EntryLogViewModel WorkEntry { get; set; }
    public ShiftViewModel Shift { get; set; }
    public WorkPackageTaskViewModel[] PackageTasks { get; set; }
    public int Affected { get; set; }
}