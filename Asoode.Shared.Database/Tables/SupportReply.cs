using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class SupportReply : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid SupportId { get; set; }
    [Required] [MaxLength(1000)] public string Message { get; set; }
    public bool Seen { get; set; }
}