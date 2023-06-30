using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class WorkPackageTask : BaseEntity
{
    public Guid UserId { get; set; }
    [Required] public Guid ListId { get; set; }
    [Required] public Guid PackageId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? ObjectiveId { get; set; }
    public Guid? SubProjectId { get; set; }
    public Guid? SeasonId { get; set; }
    public Guid? CoverId { get; set; }
    public Guid? DoneUserId { get; set; }
    public Guid? ParentId { get; set; }

    [Required] [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    [MaxLength(100)] public string GeoLocation { get; set; }
    public int Order { get; set; }
    public bool Restricted { get; set; }
    public bool VotePaused { get; set; }
    public bool VotePrivate { get; set; }
    public WorkPackageTaskVoteNecessity? VoteNecessity { get; set; }
    public WorkPackageTaskReminderType? BeginReminder { get; set; }
    public WorkPackageTaskReminderType? EndReminder { get; set; }
    public WorkPackageTaskState State { get; set; }
    public WorkPackageTaskObjectiveValue? ObjectiveValue { get; set; }
    public SortType? SubTasksSort { get; set; }
    public SortType? AttachmentsSort { get; set; }

    public TimeSpan? EstimatedTime { get; set; }
    public long? EstimatedTicks { get; set; }

    public long? EstimatedDuration
    {
        get
        {
            if (EstimatedTicks.HasValue) return EstimatedTicks.Value;
            EstimatedTicks = EstimatedTime?.Ticks;
            return EstimatedTicks;
        }
    }

    public DateTime? ArchivedAt { get; set; }
    public DateTime? DueAt { get; set; }
    public DateTime? BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime? DoneAt { get; set; }
    public DateTime? LastDuePassedNotified { get; set; }
    public DateTime? LastEndPassedNotified { get; set; }

    public WorkPackageTaskDto ToDto()
    {
        return new WorkPackageTaskDto
        {
            Description = Description,
            Order = Order,
            Restricted = Restricted,
            State = State,
            Title = Title,
            ArchivedAt = ArchivedAt,
            AttachmentsSort = AttachmentsSort,
            BeginAt = BeginAt,
            BeginReminder = BeginReminder,
            CoverId = CoverId,
            DoneAt = DoneAt,
            DueAt = DueAt,
            EndAt = EndAt,
            ListId = ListId,
            EndReminder = EndReminder,
            GeoLocation = GeoLocation,
            ObjectiveValue = ObjectiveValue,
            PackageId = PackageId,
            ParentId = ParentId,
            ProjectId = ProjectId,
            SeasonId = SeasonId,
            Id = Id,
            UserId = UserId,
            VoteNecessity = VoteNecessity,
            VotePaused = VotePaused,
            VotePrivate = VotePrivate,
            DoneUserId = DoneUserId,
            SubProjectId = SubProjectId,
            CreatedAt = CreatedAt,
            SubTasksSort = SubTasksSort,
            EstimatedTime = EstimatedTime?.Ticks,
            
            // TODO: fix this
            // Attachments = Attachments,
            // Comments = Comments,
            // Labels = Labels,
            // Members = Members,
            // Votes = Votes,
            // Watching = Watching,
            // AttachmentCount = AttachmentCount,
            // CommentCount = CommentCount,
            // DownVotes = DownVotes,
            // HasDescription = HasDescription,
            // ListName = ListName,
            // SubTasks = SubTasks,
            // TargetCount = TargetCount,
            // TimeSpents = TimeSpents,
            // UpdatedAt = UpdatedAt,
            // UpVotes = UpVotes,
            // SubTasksCount = SubTasksCount,
            // SubTasksDone = SubTasksDone,
            // TimeSpent = TimeSpent,
        };
    }
}