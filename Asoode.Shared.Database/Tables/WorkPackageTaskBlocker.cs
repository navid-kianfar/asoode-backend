using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class WorkPackageTaskBlocker : BaseEntity
{
    [Required] public Guid BlockerPackageId { get; set; }
    [Required] public Guid BlockerProjectId { get; set; }
    public Guid? BlockerSubProjectId { get; set; }
    [Required] public Guid BlockerId { get; set; }

    [Required] public Guid PackageId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    [Required] public Guid TaskId { get; set; }

    public DateTime? ReleasedAt { get; set; }
}