using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Communication;

public record ChannelDto : BaseDto
{
    public DateTime? ArchivedAt { get; set; }
    public string Title { get; set; }
    public ChannelType Type { get; set; }
    public Guid UserId { get; set; }
    public Guid RootId { get; set; }
}