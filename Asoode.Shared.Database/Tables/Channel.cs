using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Channel : BaseEntity
{
    public DateTime? ArchivedAt { get; set; }
    [Required] [MaxLength(1000)] public string Title { get; set; }
    public ChannelType Type { get; set; }
    [Required] public Guid UserId { get; set; }
    public Guid RootId { get; set; }
}