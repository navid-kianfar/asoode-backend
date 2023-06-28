using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class WorkPackage : BaseEntity
{
    public WorkPackage()
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
    }

    [Required] public Guid UserId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }

    public DateTime? BeginAt { get; set; }
    public bool Premium { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime? ActualBeginAt { get; set; }
    public DateTime? ActualEndAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    [MaxLength(100)] public string Color { get; set; }
    public bool DarkColor { get; set; }

    public WorkPackageTaskVisibility TaskVisibility { get; set; }
    public SortType ListsSort { get; set; }
    public SortType TasksSort { get; set; }
    public SortType SubTasksSort { get; set; }
    public SortType AttachmentsSort { get; set; }

    public WorkPackageCommentPermission CommentPermission { get; set; }

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

    public int Order { get; set; }

    #region Allow

    [DefaultValue(true)] public bool AllowAttachment { get; set; }
    [DefaultValue(true)] public bool AllowBlockingBoardTasks { get; set; }
    [DefaultValue(true)] public bool AllowComments { get; set; }
    [DefaultValue(true)] public bool AllowCustomField { get; set; }
    [DefaultValue(true)] public bool AllowEndAt { get; set; }
    [DefaultValue(true)] public bool AllowEstimatedTime { get; set; }
    [DefaultValue(true)] public bool AllowGeoLocation { get; set; }
    [DefaultValue(true)] public bool AllowLabels { get; set; }
    [DefaultValue(true)] public bool AllowMembers { get; set; }
    [DefaultValue(true)] public bool AllowPoll { get; set; }
    [DefaultValue(true)] public bool AllowSegments { get; set; }
    [DefaultValue(true)] public bool AllowState { get; set; }
    [DefaultValue(true)] public bool AllowTimeSpent { get; set; }

    #endregion
}