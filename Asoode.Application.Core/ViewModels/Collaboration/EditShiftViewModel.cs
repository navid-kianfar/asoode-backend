using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.Collaboration;

public class EditShiftViewModel
{
    [MaxLength(500)] [Required] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    [Required] public int WorkingHours { get; set; }
    [Required] public int RestHours { get; set; }
    [Required] public float PenaltyRate { get; set; }
    [Required] public float RewardRate { get; set; }
    [Required] public ShiftType Type { get; set; }
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