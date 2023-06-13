using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Collaboration;

public class ShiftViewModel : BaseViewModel
{
    public int Index { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int WorkingHours { get; set; }
    public int RestHours { get; set; }
    public float PenaltyRate { get; set; }
    public float RewardRate { get; set; }
    public string Start { get; set; }
    public string End { get; set; }
    public string Float { get; set; }
    public ShiftType Type { get; set; }

    public bool Saturday { get; set; }
    public bool Sunday { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
}