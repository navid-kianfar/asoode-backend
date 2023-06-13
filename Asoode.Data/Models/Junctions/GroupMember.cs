using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class GroupMember : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public AccessType Access { get; set; }
    public Guid RootId { get; set; }
    public int Level { get; set; }
}