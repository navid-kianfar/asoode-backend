using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class WorkPackageView : BaseEntity
{
    [Required] public Guid UserId { get; set; }
    public Guid? PackageId { get; set; }
    public Guid? FieldId { get; set; }
    public int Order { get; set; }
    public int Width { get; set; }
    public WorkPackageStaticFields Type { get; set; }
}