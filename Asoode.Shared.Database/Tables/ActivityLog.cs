using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class ActivityLog : BaseEntity
{
    [Required] [MaxLength(2000)] public string Description { get; set; }
    public Guid RecordId { get; set; }
    public ActivityType Type { get; set; }
    public Guid UserId { get; set; }
}