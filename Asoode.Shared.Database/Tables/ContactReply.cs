using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class ContactReply : BaseEntity
{
    public Guid ContactId { get; set; }
    public Guid UserId { get; set; }
    [Required] [MaxLength(2000)] public string Message { get; set; }
}