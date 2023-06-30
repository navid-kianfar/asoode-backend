using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Communication;

public record ConversationDto : BaseDto
{
    public Guid ChannelId { get; set; }
    public string Message { get; set; }
    public string Path { get; set; }
    public Guid? ReplyId { get; set; }
    public Guid? UploadId { get; set; }
    public ConversationType Type { get; set; }
    public Guid? UserId { get; set; }
    public bool FromBot { get; set; }
    public UploadDto Upload { get; set; }
}