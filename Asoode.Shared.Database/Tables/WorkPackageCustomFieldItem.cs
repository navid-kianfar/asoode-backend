using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class WorkPackageCustomFieldItem : BaseEntity
{
    [Required] public Guid PackageId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }

    [Required] public Guid CustomFieldId { get; set; }
    [MaxLength(10)] public string Color { get; set; }
    public int Order { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
}