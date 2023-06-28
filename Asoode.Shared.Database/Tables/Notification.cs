using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class Notification : BaseEntity
{
    [MaxLength(250)] public string Avatar { get; set; }
    [MaxLength(2000)] public string Data { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Url { get; set; }
    public Guid UserId { get; set; }
}