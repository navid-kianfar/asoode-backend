using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.Communication;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Logging;

public record ActivityLogDto
{
    public ActivityType Type { get; set; }
    public Guid UserId { get; set; }
    public GroupDto Group { get; set; }
    public ProjectDto Project { get; set; }
    public WorkPackageDto WorkPackage { get; set; }
    public MemberInfoDto User { get; set; }
    public PendingInvitationDto[] Pendings { get; set; }
    public GroupMemberDto[] GroupMembers { get; set; }
    public ProjectMemberDto[] ProjectMembers { get; set; }
    public WorkPackageMemberDto[] WorkPackageMembers { get; set; }
    public Guid[] UserIds { get; set; }

    public PendingInvitationDto Pending { get; set; }
    public GroupMemberDto GroupMember { get; set; }
    public ProjectMemberDto ProjectMember { get; set; }
    public WorkPackageMemberDto WorkPackageMember { get; set; }
    public ConversationDto Conversation { get; set; }
    public SubProjectDto SubProject { get; set; }
    public ProjectSeasonDto Season { get; set; }
    public WorkPackageMemberSettingDto PackageUserSetting { get; set; }
    public WorkPackageLabelDto PackageLabel { get; set; }
    public WorkPackageObjectiveDto Objective { get; set; }
    public WorkPackageListDto PackageList { get; set; }
    public WorkPackageTaskDto PackageTask { get; set; }
    public WorkPackageTaskVoteDto PackageTaskVote { get; set; }
    public WorkPackageTaskTimeDto PackageTaskTime { get; set; }
    public WorkPackageTaskInteractionDto PackageTaskInteraction { get; set; }
    public WorkPackageTaskCommentDto PackageTaskComment { get; set; }
    public WorkPackageTaskLabelDto PackageTaskLabel { get; set; }
    public WorkPackageTaskAttachmentDto PackageTaskAttachment { get; set; }
    public WorkPackageTaskMemberDto WorkPackageTaskMember { get; set; }
    public DeviceDto Device { get; set; }
    public WorkPackageDto WorkPackage2 { get; set; }
    public EntryLogDto WorkEntry { get; set; }
    public ShiftDto Shift { get; set; }
    public WorkPackageTaskDto[] PackageTasks { get; set; }
    public int Affected { get; set; }
}