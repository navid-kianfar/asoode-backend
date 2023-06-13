using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class PendingInvitation : BaseEntity
{
    public string Identifier { get; set; }
    public Guid RecordId { get; set; }
    public AccessType Access { get; set; }
    public PendingInvitationType Type { get; set; }
    public bool Canceled { get; set; }
}