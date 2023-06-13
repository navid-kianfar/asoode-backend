using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Storage;
using Asoode.Data.Models.Junctions;

namespace Asoode.Data.Models.Base;

public static class ProjectExtensions
{
    public static ProjectViewModel ToViewModel(this Project project,
        ProjectSeasonViewModel[] seasons = null,
        ProjectMemberViewModel[] members = null,
        WorkPackageViewModel[] workPackages = null,
        SubProjectViewModel[] subs = null,
        PendingInvitationViewModel[] pending = null,
        int attachmentSize = 0)
    {
        return new ProjectViewModel
        {
            Pending = pending ?? new PendingInvitationViewModel[0],
            Description = project.Description,
            Id = project.Id,
            Title = project.Title,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            UserId = project.UserId,
            Complex = project.Complex,
            Premium = project.Premium,
            Seasons = seasons,
            Members = members,
            WorkPackages = workPackages,
            SubProjects = subs ?? new SubProjectViewModel[0],
            Template = project.Template,
            ArchivedAt = project.ArchivedAt,
            AttachmentSize = attachmentSize
        };
    }

    public static WorkPackageTaskTimeViewModel ToViewModel(this WorkPackageTaskTime time)
    {
        return new WorkPackageTaskTimeViewModel
        {
            Id = time.Id,
            Begin = time.Begin,
            End = time.End,
            Manual = time.Manual,
            CreatedAt = time.CreatedAt,
            PackageId = time.PackageId,
            ProjectId = time.ProjectId,
            TaskId = time.TaskId,
            UpdatedAt = time.UpdatedAt,
            UserId = time.UserId,
            SubProjectId = time.SubProjectId
        };
    }

    public static WorkPackageTaskVoteViewModel ToViewModel(this WorkPackageTaskVote vote)
    {
        return new WorkPackageTaskVoteViewModel
        {
            Id = vote.Id,
            Vote = vote.Vote,
            CreatedAt = vote.CreatedAt,
            PackageId = vote.PackageId,
            ProjectId = vote.ProjectId,
            TaskId = vote.TaskId,
            UpdatedAt = vote.UpdatedAt,
            UserId = vote.UserId,
            SubProjectId = vote.SubProjectId
        };
    }

    public static WorkPackageTaskAttachmentViewModel ToViewModel(this WorkPackageTaskAttachment attachment)
    {
        return new WorkPackageTaskAttachmentViewModel
        {
            Description = attachment.Description,
            Id = attachment.Id,
            Path = attachment.Path,
            ThumbnailPath = attachment.ThumbnailPath,
            Title = attachment.Title,
            Type = attachment.Type,
            CreatedAt = attachment.CreatedAt,
            IsCover = attachment.IsCover,
            PackageId = attachment.PackageId,
            ProjectId = attachment.ProjectId,
            TaskId = attachment.TaskId,
            UpdatedAt = attachment.UpdatedAt,
            UploadId = attachment.UploadId,
            UserId = attachment.UserId,
            SubProjectId = attachment.SubProjectId
        };
    }

    public static UploadViewModel ToViewModel(this Upload upload)
    {
        return new UploadViewModel
        {
            Directory = upload.Directory,
            Extension = upload.Extension,
            Id = upload.Id,
            Name = upload.Name,
            Path = upload.Path,
            Public = upload.Public,
            Section = upload.Section,
            Size = upload.Size,
            Type = upload.Type,
            CreatedAt = upload.CreatedAt,
            RecordId = upload.RecordId,
            ThumbnailPath = upload.ThumbnailPath,
            UserId = upload.UserId
        };
    }

    public static WorkPackageViewModel ToViewModel(
        this WorkPackage wp,
        WorkPackageMemberViewModel[] members = null,
        PendingInvitationViewModel[] pending = null,
        WorkPackageObjectiveViewModel[] objectives = null,
        WorkPackageListViewModel[] lists = null,
        WorkPackageTaskViewModel[] tasks = null,
        WorkPackageCustomFieldViewModel[] customFields = null,
        WorkPackageCustomFieldItemViewModel[] customFieldItems = null,
        WorkPackageLabelViewModel[] labels = null,
        WorkPackageProgressViewModel progress = null
    )
    {
        return new WorkPackageViewModel
        {
            Labels = labels ?? new WorkPackageLabelViewModel[0],
            CustomFieldsItems = customFieldItems ?? new WorkPackageCustomFieldItemViewModel[0],
            CustomFields = customFields ?? new WorkPackageCustomFieldViewModel[0],
            Description = wp.Description,
            Id = wp.Id,
            Title = wp.Title,
            CreatedAt = wp.CreatedAt,
            ProjectId = wp.ProjectId,
            UserId = wp.UserId,
            Color = wp.Color,
            AllowAttachment = wp.AllowAttachment,
            AllowComments = wp.AllowComments,
            AllowLabels = wp.AllowLabels,
            AllowMembers = wp.AllowMembers,
            AllowPoll = wp.AllowPoll,
            AllowSegments = wp.AllowSegments,
            AllowState = wp.AllowState,
            ArchivedAt = wp.ArchivedAt,
            BeginAt = wp.BeginAt,
            CommentPermission = wp.CommentPermission,
            DarkColor = wp.DarkColor,
            EndAt = wp.EndAt,
            ActualBeginAt = wp.ActualBeginAt,
            ActualEndAt = wp.ActualEndAt,
            AllowCustomField = wp.AllowCustomField,
            AllowEndAt = wp.AllowEndAt,
            AllowEstimatedTime = wp.AllowEstimatedTime,
            AllowGeoLocation = wp.AllowGeoLocation,
            AllowTimeSpent = wp.AllowTimeSpent,
            SubProjectId = wp.SubProjectId,
            AllowBlockingBoardTasks = wp.AllowBlockingBoardTasks,
            TaskVisibility = wp.TaskVisibility,
            Members = members ?? new WorkPackageMemberViewModel[0],
            Objectives = objectives ?? new WorkPackageObjectiveViewModel[0],
            Lists = lists ?? new WorkPackageListViewModel[0],
            Tasks = tasks ?? new WorkPackageTaskViewModel[0],
            Pending = pending ?? new PendingInvitationViewModel[0],
            Progress = progress,
            Order = wp.Order,
            PermissionComment = wp.PermissionComment,
            PermissionEditAttachment = wp.PermissionEditAttachment,
            PermissionCreateAttachment = wp.PermissionCreateAttachment,
            PermissionAssignMembers = wp.PermissionAssignMembers,
            PermissionAssignLabels = wp.PermissionAssignLabels,
            PermissionChangeTaskState = wp.PermissionChangeTaskState,
            PermissionEditTask = wp.PermissionEditTask,
            PermissionArchiveTask = wp.PermissionArchiveTask,
            PermissionCreateTask = wp.PermissionCreateTask,
            PermissionArchiveList = wp.PermissionArchiveList,
            PermissionEditList = wp.PermissionEditList,
            PermissionCreateList = wp.PermissionCreateList,
            ListsSort = wp.ListsSort,
            AttachmentsSort = wp.AttachmentsSort,
            TasksSort = wp.TasksSort,
            SubTasksSort = wp.SubTasksSort
        };
    }

    public static WorkPackageMemberViewModel ToViewModel(this WorkPackageMember member)
    {
        return new WorkPackageMemberViewModel
        {
            Access = member.Access,
            Id = member.Id,
            CreatedAt = member.CreatedAt,
            PackageId = member.PackageId,
            UpdatedAt = member.UpdatedAt,
            RecordId = member.RecordId,
            IsGroup = member.IsGroup
        };
    }

    public static ProjectSeasonViewModel ToViewModel(this ProjectSeason season)
    {
        return new ProjectSeasonViewModel
        {
            Description = season.Description,
            Id = season.Id,
            Title = season.Title,
            CreatedAt = season.CreatedAt,
            ProjectId = season.ProjectId,
            UserId = season.UserId
        };
    }

    public static WorkPackageMemberSettingViewModel ToViewModel(this WorkPackageMemberSetting setting)
    {
        return new WorkPackageMemberSettingViewModel
        {
            Id = setting.Id,
            CreatedAt = setting.CreatedAt,
            ProjectId = setting.ProjectId,
            UserId = setting.UserId,
            PackageId = setting.PackageId,
            ReceiveNotification = setting.ReceiveNotification,
            UpdatedAt = setting.UpdatedAt,
            ShowTotalCards = setting.ShowTotalCards
        };
    }

    public static ProjectMemberViewModel ToViewModel(this ProjectMember member, User user = null)
    {
        return new ProjectMemberViewModel
        {
            Access = member.Access,
            Id = member.Id,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt,
            ProjectId = member.ProjectId,
            IsGroup = member.IsGroup,
            RecordId = member.RecordId,
            Member = user?.ToViewModel()
        };
    }

    public static WorkPackageListViewModel ToViewModel(this WorkPackageList list)
    {
        return new WorkPackageListViewModel
        {
            Color = list.Color,
            Order = list.Order,
            Title = list.Title,
            DarkColor = list.DarkColor,
            PackageId = list.PackageId,
            Id = list.Id,
            CreatedAt = list.CreatedAt,
            UpdatedAt = list.UpdatedAt,
            ArchivedAt = list.ArchivedAt,
            Restricted = list.Restricted,
            Kanban = list.Kanban
        };
    }

    public static WorkPackageObjectiveViewModel ToViewModel(this WorkPackageObjective o)
    {
        return new WorkPackageObjectiveViewModel
        {
            Description = o.Description,
            Level = o.Level,
            Order = o.Order,
            Title = o.Title,
            Type = o.Type,
            PackageId = o.PackageId,
            ParentId = o.ParentId,
            Id = o.Id
        };
    }

    public static WorkPackageTaskViewModel ToViewModel(
        this WorkPackageTask task,
        WorkPackageTaskMemberViewModel[] members = null,
        WorkPackageTaskLabelViewModel[] labels = null,
        WorkPackageTaskCommentViewModel[] comments = null
    )
    {
        return new WorkPackageTaskViewModel
        {
            Description = task.Description,
            Order = task.Order,
            Restricted = task.Restricted,
            State = task.State,
            Title = task.Title,
            ArchivedAt = task.ArchivedAt,
            BeginAt = task.BeginAt,
            BeginReminder = task.BeginReminder,
            CoverId = task.CoverId,
            DoneAt = task.DoneAt,
            DueAt = task.DueAt,
            EndAt = task.EndAt,
            EndReminder = task.EndReminder,
            EstimatedTime = task.EstimatedDuration,
            GeoLocation = task.GeoLocation,
            ListId = task.ListId,
            ObjectiveValue = task.ObjectiveValue,
            PackageId = task.PackageId,
            ParentId = task.ParentId,
            ProjectId = task.ProjectId,
            SeasonId = task.SeasonId,
            UserId = task.UserId,
            VoteNecessity = task.VoteNecessity,
            VotePaused = task.VotePaused,
            VotePrivate = task.VotePrivate,
            DoneUserId = task.DoneUserId,
            SubProjectId = task.SubProjectId,
            Members = members ?? new WorkPackageTaskMemberViewModel[0],
            Labels = labels ?? new WorkPackageTaskLabelViewModel[0],
            Comments = comments ?? new WorkPackageTaskCommentViewModel[0],
            Id = task.Id,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            SubTasksSort = task.SubTasksSort,
            AttachmentsSort = task.AttachmentsSort
        };
    }

    public static AdvancedPlayerCommentViewModel ToViewModel(this AdvancedPlayerComment comment)
    {
        return new AdvancedPlayerCommentViewModel
        {
            Id = comment.Id,
            Message = comment.Message,
            Payload = comment.Payload,
            AttachmentId = comment.AttachmentId,
            CreatedAt = comment.CreatedAt,
            EndFrame = comment.EndFrame,
            StartFrame = comment.StartFrame,
            UpdatedAt = comment.UpdatedAt
        };
    }

    public static AdvancedPlayerShapeViewModel ToViewModel(this AdvancedPlayerShape comment)
    {
        return new AdvancedPlayerShapeViewModel
        {
            Id = comment.Id,
            AttachmentId = comment.AttachmentId,
            CreatedAt = comment.CreatedAt,
            EndFrame = comment.EndFrame,
            StartFrame = comment.StartFrame,
            UpdatedAt = comment.UpdatedAt
        };
    }

    public static WorkPackageTaskMemberViewModel ToViewModel(this WorkPackageTaskMember member)
    {
        return new WorkPackageTaskMemberViewModel
        {
            Id = member.Id,
            CreatedAt = member.CreatedAt,
            PackageId = member.PackageId,
            TaskId = member.TaskId,
            UpdatedAt = member.UpdatedAt,
            RecordId = member.RecordId,
            IsGroup = member.IsGroup
        };
    }

    public static WorkPackageTaskInteractionViewModel ToViewModel(this WorkPackageTaskInteraction interaction)
    {
        return new WorkPackageTaskInteractionViewModel
        {
            Id = interaction.Id,
            Vote = interaction.Vote,
            Watching = interaction.Watching,
            CreatedAt = interaction.CreatedAt,
            LastView = interaction.LastView,
            PackageId = interaction.PackageId,
            TaskId = interaction.TaskId,
            UpdatedAt = interaction.UpdatedAt
        };
    }

    public static WorkPackageTaskCommentViewModel ToViewModel(this WorkPackageTaskComment comment)
    {
        return new WorkPackageTaskCommentViewModel
        {
            Id = comment.Id,
            CreatedAt = comment.CreatedAt,
            TaskId = comment.TaskId,
            UpdatedAt = comment.UpdatedAt,
            UserId = comment.UserId,
            Message = comment.Message,
            Private = comment.Private,
            ReplyId = comment.ReplyId,
            PackageId = comment.PackageId,
            ProjectId = comment.ProjectId
        };
    }

    public static WorkPackageTaskLabelViewModel ToViewModel(this WorkPackageTaskLabel label)
    {
        return new WorkPackageTaskLabelViewModel
        {
            Id = label.Id,
            CreatedAt = label.CreatedAt,
            PackageId = label.PackageId,
            TaskId = label.TaskId,
            UpdatedAt = label.UpdatedAt,
            LabelId = label.LabelId
        };
    }

    public static SubProjectViewModel ToViewModel(this SubProject sub)
    {
        return new SubProjectViewModel
        {
            Description = sub.Description,
            Id = sub.Id,
            Level = sub.Level,
            Order = sub.Order,
            Title = sub.Title,
            CreatedAt = sub.CreatedAt,
            ParentId = sub.ParentId,
            ProjectId = sub.ProjectId,
            UpdatedAt = sub.UpdatedAt,
            UserId = sub.UserId
        };
    }

    public static WorkPackageCustomFieldViewModel ToViewModel(this WorkPackageCustomField customField)
    {
        return new WorkPackageCustomFieldViewModel
        {
            Show = customField.Show,
            Title = customField.Title,
            Type = customField.Type,
            PackageId = customField.PackageId,
            ProjectId = customField.ProjectId,
            SubProjectId = customField.SubProjectId
        };
    }

    public static WorkPackageCustomFieldItemViewModel ToViewModel(this WorkPackageCustomFieldItem customFieldItem)
    {
        return new WorkPackageCustomFieldItemViewModel
        {
            Color = customFieldItem.Color,
            Order = customFieldItem.Order,
            Title = customFieldItem.Title,
            PackageId = customFieldItem.PackageId,
            ProjectId = customFieldItem.ProjectId,
            CustomFieldId = customFieldItem.CustomFieldId,
            SubProjectId = customFieldItem.SubProjectId
        };
    }

    public static WorkPackageLabelViewModel ToViewModel(this WorkPackageLabel label)
    {
        return new WorkPackageLabelViewModel
        {
            Color = label.Color,
            Title = label.Title,
            DarkColor = label.DarkColor,
            PackageId = label.PackageId,
            Id = label.Id,
            CreatedAt = label.CreatedAt,
            UpdatedAt = label.UpdatedAt
        };
    }
}