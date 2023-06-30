using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class WorkPackageTaskLabel : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid LabelId { get; set; }
    public Guid PackageId { get; set; }

    public WorkPackageTaskLabelDto ToDto()
    {
        return new WorkPackageTaskLabelDto
        {
            PackageId = PackageId,
            LabelId = LabelId,
            TaskId = TaskId,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }
}