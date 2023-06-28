using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class PendingInvitation : BaseEntity
{
    public string Identifier { get; set; }
    public Guid RecordId { get; set; }
    public AccessType Access { get; set; }
    public PendingInvitationType Type { get; set; }
    public bool Canceled { get; set; }
}