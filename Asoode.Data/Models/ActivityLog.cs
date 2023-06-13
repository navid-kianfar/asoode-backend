using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class ActivityLog : BaseEntity
{
    [Required] [MaxLength(2000)] public string Description { get; set; }
    public Guid RecordId { get; set; }
    public ActivityType Type { get; set; }
    public Guid UserId { get; set; }
}