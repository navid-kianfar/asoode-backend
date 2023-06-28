using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class WorkPackageTaskLabel : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid LabelId { get; set; }
    public Guid PackageId { get; set; }
}