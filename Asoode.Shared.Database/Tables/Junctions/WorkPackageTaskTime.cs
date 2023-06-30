using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class WorkPackageTaskTime : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public DateTime Begin { get; set; }
    public DateTime? End { get; set; }
    public bool Manual { get; set; }

    public WorkPackageTaskTimeDto ToDto()
    {
        return new WorkPackageTaskTimeDto
        {
            Begin = Begin,
            End = End,
            PackageId = PackageId,
            ProjectId = ProjectId,
            SubProjectId = SubProjectId,
            Manual = Manual,
            TaskId = TaskId,
            UserId = UserId,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }
}