using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class Conversation : BaseEntity
{
    public Guid ChannelId { get; set; }
    [MaxLength(2000)] public string Message { get; set; }
    [MaxLength(500)] public string Path { get; set; }
    public Guid? ReplyId { get; set; }
    public Guid? UploadId { get; set; }
    public ConversationType Type { get; set; }
    public Guid? UserId { get; set; }
}