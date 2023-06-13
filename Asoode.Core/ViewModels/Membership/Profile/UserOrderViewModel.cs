using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Membership.Profile;

public class UserOrderViewModel : BaseViewModel
{
    public int Index { get; set; }
    public string Title { get; set; }
    public double Amount { get; set; }
    public DateTime? DueAt { get; set; }
    public int PreviousDebt { get; set; }
    public OrderStatus Status { get; set; }
}