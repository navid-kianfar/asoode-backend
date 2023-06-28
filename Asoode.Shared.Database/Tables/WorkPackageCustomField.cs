using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class WorkPackageCustomField : BaseEntity
{
    [Required] public Guid PackageId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public bool Show { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    public WorkPackageCustomFieldType Type { get; set; }
}