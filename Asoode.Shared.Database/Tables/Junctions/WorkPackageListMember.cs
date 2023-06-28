using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class WorkPackageListMember : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ListId { get; set; }

    public bool Chart { get; set; }
}