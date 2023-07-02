using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageDto : BaseDto
{
    public WorkPackageMemberDto[] Members { get; set; }
    public WorkPackageObjectiveDto[] Objectives { get; set; }
    public WorkPackageListDto[] Lists { get; set; }
    public WorkPackageTaskDto[] Tasks { get; set; }
    public WorkPackageCustomFieldDto[] CustomFields { get; set; }
    public WorkPackageCustomFieldItemDto[] CustomFieldsItems { get; set; }
    public WorkPackageLabelDto[] Labels { get; set; }
    public PendingInvitationDto[] Pending { get; set; }
    public WorkPackageMemberSettingDto UserSetting { get; set; }
    public WorkPackageTaskVisibility TaskVisibility { get; set; }
    public WorkPackageProgressDto Progress { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime? ActualBeginAt { get; set; }
    public DateTime? ActualEndAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string Color { get; set; }
    public bool DarkColor { get; set; }
    public WorkPackageCommentPermission CommentPermission { get; set; }
    public bool AllowAttachment { get; set; }
    public bool AllowBlockingBoardTasks { get; set; }
    public bool AllowComments { get; set; }
    public bool AllowCustomField { get; set; }
    public bool AllowEndAt { get; set; }
    public bool AllowEstimatedTime { get; set; }
    public bool AllowGeoLocation { get; set; }
    public bool AllowLabels { get; set; }
    public bool AllowMembers { get; set; }
    public bool AllowPoll { get; set; }
    public bool AllowSegments { get; set; }
    public bool AllowState { get; set; }
    public bool AllowTimeSpent { get; set; }
    public int Order { get; set; }


    public bool PermissionComment { get; set; }
    public bool PermissionEditAttachment { get; set; }
    public bool PermissionCreateAttachment { get; set; }
    public bool PermissionAssignMembers { get; set; }
    public bool PermissionAssignLabels { get; set; }
    public bool PermissionChangeTaskState { get; set; }
    public bool PermissionEditTask { get; set; }
    public bool PermissionArchiveTask { get; set; }
    public bool PermissionCreateTask { get; set; }
    public bool PermissionArchiveList { get; set; }
    public bool PermissionEditList { get; set; }
    public bool PermissionCreateList { get; set; }
    public SortType ListsSort { get; set; }
    public SortType TasksSort { get; set; }
    public SortType SubTasksSort { get; set; }
    public SortType AttachmentsSort { get; set; }

    public ExplorerFolderDto ToExplorerDto(string parentType)
    {
        return new ExplorerFolderDto
        {
            Name = Title,
            Path = "/package/" + Id,
            Parent = $"/project/{parentType}/{ProjectId}",
            CreatedAt = CreatedAt
        };
    }
}