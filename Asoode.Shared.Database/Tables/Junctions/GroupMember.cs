using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class GroupMember : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public AccessType Access { get; set; }
    public Guid RootId { get; set; }
    public int Level { get; set; }
}