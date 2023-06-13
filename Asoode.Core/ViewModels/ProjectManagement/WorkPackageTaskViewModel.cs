using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class WorkPackageTaskViewModel : BaseViewModel
{
    public WorkPackageTaskViewModel()
    {
        Members = new WorkPackageTaskMemberViewModel[0];
        Labels = new WorkPackageTaskLabelViewModel[0];
        Comments = new WorkPackageTaskCommentViewModel[0];
        Attachments = new WorkPackageTaskAttachmentViewModel[0];
        SubTasks = new WorkPackageTaskViewModel[0];
        Votes = new WorkPackageTaskVoteViewModel[0];
        TimeSpents = new WorkPackageTaskTimeViewModel[0];
    }

    public SortType? SubTasksSort { get; set; }
    public SortType? AttachmentsSort { get; set; }
    public WorkPackageTaskMemberViewModel[] Members { get; set; }
    public WorkPackageTaskLabelViewModel[] Labels { get; set; }
    public WorkPackageTaskCommentViewModel[] Comments { get; set; }
    public WorkPackageTaskAttachmentViewModel[] Attachments { get; set; }
    public WorkPackageTaskViewModel[] SubTasks { get; set; }
    public WorkPackageTaskVoteViewModel[] Votes { get; set; }
    public WorkPackageTaskTimeViewModel[] TimeSpents { get; set; }
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
}