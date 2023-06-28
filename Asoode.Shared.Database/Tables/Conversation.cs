using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Conversation : BaseEntity
{
    public Guid ChannelId { get; set; }
    [MaxLength(2000)] public string Message { get; set; }
    [MaxLength(500)] public string Path { get; set; }
    public Guid? ReplyId { get; set; }
    public Guid? UploadId { get; set; }
    public ConversationType Type { get; set; }
    public Guid? UserId { get; set; }
}