using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class WorkPackageTask : BaseEntity
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
    }
}