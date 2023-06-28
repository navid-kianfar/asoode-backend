using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class WorkPackageTaskVote : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public bool Vote { get; set; }
}