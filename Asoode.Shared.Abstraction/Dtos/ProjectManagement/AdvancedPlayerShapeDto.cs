using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record AdvancedPlayerShapeDto : BaseDto
{
    public Guid AttachmentId { get; set; }
    public int StartFrame { get; set; }
    public int EndFrame { get; set; }
}