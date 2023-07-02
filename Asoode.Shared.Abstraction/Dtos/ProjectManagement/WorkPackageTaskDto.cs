using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageTaskDto : BaseDto
{
    public SortType? SubTasksSort { get; set; }
    public SortType? AttachmentsSort { get; set; }
    public WorkPackageTaskMemberDto[] Members { get; set; }
    public WorkPackageTaskLabelDto[] Labels { get; set; }
    public WorkPackageTaskCommentDto[] Comments { get; set; }
    public WorkPackageTaskAttachmentDto[] Attachments { get; set; }
    public WorkPackageTaskDto[] SubTasks { get; set; }
    public WorkPackageTaskVoteDto[] Votes { get; set; }
    public WorkPackageTaskTimeDto[] TimeSpents { get; set; }
    public Guid UserId { get; set; }
    public Guid ListId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public Guid? SeasonId { get; set; }
    public Guid? CoverId { get; set; }
    public Guid? DoneUserId { get; set; }
    public Guid? ParentId { get; set; }
    public WorkPackageTaskReminderType? BeginReminder { get; set; }
    public WorkPackageTaskReminderType? EndReminder { get; set; }
    public WorkPackageTaskState State { get; set; }
    public WorkPackageTaskVoteNecessity? VoteNecessity { get; set; }
    public WorkPackageTaskObjectiveValue? ObjectiveValue { get; set; }
    public long? EstimatedTime { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime? DueAt { get; set; }
    public DateTime? BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime? DoneAt { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string GeoLocation { get; set; }
    public int Order { get; set; }
    public bool Restricted { get; set; }
    public bool VotePaused { get; set; }
    public bool VotePrivate { get; set; }

    public int AttachmentCount { get; set; }
    public int CommentCount { get; set; }
    public int TimeSpent { get; set; }
    public int TargetCount { get; set; }
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public int SubTasksCount { get; set; }
    public int SubTasksDone { get; set; }
    public bool HasDescription { get; set; }
    public string ListName { get; set; }
    public bool Watching { get; set; }

    public ExplorerFolderDto ToExplorerDto(string parentPath)
    {
        return new ExplorerFolderDto
        {
            Parent = parentPath,
            Path = "/task/" + Id,
            Name = Title,
            CreatedAt = CreatedAt
        };
    }
}