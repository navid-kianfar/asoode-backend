using System;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class WorkPackageTaskInteraction : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid PackageId { get; set; }
    public DateTime? LastView { get; set; }
    public bool? Watching { get; set; }
    public bool? Vote { get; set; }
    public Guid UserId { get; set; }
}