using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class AdvancedPlayerComment : BaseEntity
{
    public Guid AttachmentId { get; set; }
    public Guid UserId { get; set; }
    public float StartFrame { get; set; }
    public float? EndFrame { get; set; }
    [MaxLength(1000)] public string Message { get; set; }
    [MaxLength(1000)] public string Payload { get; set; }
}