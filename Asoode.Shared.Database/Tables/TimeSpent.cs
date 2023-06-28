using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

public class TimeSpent : BaseEntity
{
    public DateTime? BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? TimeOffId { get; set; }
    public TimeSpentType Type { get; set; }
    [Required] public Guid UserId { get; set; }
}