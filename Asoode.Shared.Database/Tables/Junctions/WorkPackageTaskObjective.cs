using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class WorkPackageTaskObjective : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid ObjectiveId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public WorkPackageTaskObjectiveValue? ObjectiveValue { get; set; }
}