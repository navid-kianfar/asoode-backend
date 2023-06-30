using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record AdvancedPlayerCommentDto : BaseDto
{
    public Guid AttachmentId { get; set; }

    public float StartFrame { get; set; }
    public float? EndFrame { get; set; }
    [MaxLength(1000)] public string Message { get; set; }
    [MaxLength(1000)] public string Payload { get; set; }
}