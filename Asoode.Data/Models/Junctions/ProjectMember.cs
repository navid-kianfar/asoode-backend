using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class ProjectMember : BaseEntity
{
    public bool IsGroup { get; set; }
    public Guid RecordId { get; set; }
    public Guid ProjectId { get; set; }
    public AccessType Access { get; set; }
    public bool BlockNotification { get; set; }
}