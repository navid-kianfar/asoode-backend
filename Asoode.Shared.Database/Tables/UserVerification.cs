using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class UserVerification : BaseEntity
{
    [MaxLength(10)] public string Code { get; set; }
    [MaxLength(250)] public string Email { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime LastSend { get; set; }
    [MaxLength(50)] public string PhoneNumber { get; set; }
    [Required] public Guid UserId { get; set; }
}