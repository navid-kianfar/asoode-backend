using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class WorkPackageTaskMember : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid PackageId { get; set; }
    public Guid RecordId { get; set; }
    public bool IsGroup { get; set; }
}