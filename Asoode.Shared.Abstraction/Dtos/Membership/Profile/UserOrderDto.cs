using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Profile;

public record UserOrderDto : BaseDto
{
    public int Index { get; set; }
    public string Title { get; set; }
    public double Amount { get; set; }
    public DateTime? DueAt { get; set; }
    public int PreviousDebt { get; set; }
    public OrderStatus Status { get; set; }
}