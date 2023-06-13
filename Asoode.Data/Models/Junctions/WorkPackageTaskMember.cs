using System;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class WorkPackageTaskMember : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid PackageId { get; set; }
    public Guid RecordId { get; set; }
    public bool IsGroup { get; set; }
}