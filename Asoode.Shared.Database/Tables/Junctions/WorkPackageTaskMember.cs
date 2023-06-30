using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class WorkPackageTaskMember : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid PackageId { get; set; }
    public Guid RecordId { get; set; }
    public bool IsGroup { get; set; }

    public WorkPackageTaskMemberDto ToDto()
    {
        return new WorkPackageTaskMemberDto
        {
            TaskId = TaskId,
            PackageId = PackageId,
            RecordId = RecordId,
            IsGroup = IsGroup,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt, 
        };
    }
}