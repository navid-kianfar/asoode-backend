using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class SupportContact : BaseEntity
{
    public Guid? UserId { get; set; }
    public SupportType Type { get; set; }
    [Required] [MaxLength(2)] public string Culture { get; set; }
    [Required] [MaxLength(100)] public string Email { get; set; }
    [Required] [MaxLength(250)] public string FullName { get; set; }
    [Required] [MaxLength(500)] public string Subject { get; set; }
    [Required] [MaxLength(1000)] public string Message { get; set; }
    public bool Seen { get; set; }
}