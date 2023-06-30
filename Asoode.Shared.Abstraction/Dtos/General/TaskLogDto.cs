using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record TaskLogDto
{
    public string Description { get; set; }
    public Guid RecordId { get; set; }
    public ActivityType Type { get; set; }
    public Guid UserId { get; set; }
}