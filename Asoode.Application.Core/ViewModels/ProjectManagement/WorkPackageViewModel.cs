using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageViewModel : BaseViewModel
{
    public WorkPackageViewModel()
    {
        TaskVisibility = WorkPackageTaskVisibility.Normal;
        AttachmentsSort = SortType.Manual;
        ListsSort = SortType.Manual;
        TasksSort = SortType.Manual;
        SubTasksSort = SortType.Manual;
        AttachmentsSort = SortType.Manual;
        PermissionComment = true;
        PermissionEditAttachment = true;
        PermissionCreateAttachment = true;
        PermissionAssignMembers = true;
        PermissionAssignLabels = true;
        PermissionChangeTaskState = true;
        PermissionEditTask = true;
        PermissionArchiveTask = true;
        PermissionCreateTask = true;
        PermissionArchiveList = true;
        PermissionEditList = true;
        PermissionCreateList = true;

        Members = Array.Empty<WorkPackageMemberViewModel>();
        Objectives = Array.Empty<WorkPackageObjectiveViewModel>();
        Lists = Array.Empty<WorkPackageListViewModel>();
        Tasks = Array.Empty<WorkPackageTaskViewModel>();
        Labels = Array.Empty<WorkPackageLabelViewModel>();
        CustomFields = Array.Empty<WorkPackageCustomFieldViewModel>();
        CustomFieldsItems = Array.Empty<WorkPackageCustomFieldItemViewModel>();
        Pending = Array.Empty<PendingInvitationViewModel>();
    }

    public WorkPackageMemberViewModel[] Members { get; set; }
    public WorkPackageObjectiveViewModel[] Objectives { get; set; }
    public WorkPackageListViewModel[] Lists { get; set; }
    public WorkPackageTaskViewModel[] Tasks { get; set; }
    public WorkPackageCustomFieldViewModel[] CustomFields { get; set; }
    public WorkPackageCustomFieldItemViewModel[] CustomFieldsItems { get; set; }
    public WorkPackageLabelViewModel[] Labels { get; set; }
    public PendingInvitationViewModel[] Pending { get; set; }
    public WorkPackageMemberSettingViewModel UserSetting { get; set; }
    public WorkPackageTaskVisibility TaskVisibility { get; set; }
    public WorkPackageProgressViewModel Progress { get; set; }
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
}