using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class WorkingTime : BaseEntity
{
    [Required] public DateTime BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
    [Required] public Guid GroupId { get; set; }
    [Required] public Guid UserId { get; set; }
}