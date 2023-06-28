using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Shift : BaseEntity
{
    [Required] public Guid GroupId { get; set; }
    [Required] public Guid UserId { get; set; }
    [MaxLength(500)] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    public int WorkingHours { get; set; }
    public int RestHours { get; set; }
    public float PenaltyRate { get; set; }
    public float RewardRate { get; set; }
    public ShiftType Type { get; set; }
    public TimeSpan? Start { get; set; }
    public TimeSpan? End { get; set; }
    public TimeSpan? Float { get; set; }

    public bool Saturday { get; set; }
    public bool Sunday { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
}