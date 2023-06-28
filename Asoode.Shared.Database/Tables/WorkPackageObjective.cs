using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class WorkPackageObjective : BaseEntity
{
    [Required] public Guid PackageId { get; set; }
    public Guid? ParentId { get; set; }
    public int? Order { get; set; }
    public int? Level { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    public WorkPackageObjectiveType Type { get; set; }
}