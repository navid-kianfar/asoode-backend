using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class WorkPackageCustomFieldItem : BaseEntity
{
    [Required] public Guid PackageId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }

    [Required] public Guid CustomFieldId { get; set; }
    [MaxLength(10)] public string Color { get; set; }
    public int Order { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
}