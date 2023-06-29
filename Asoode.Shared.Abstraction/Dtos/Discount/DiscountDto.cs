using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Discount;

public record DiscountDto : BaseDto
{
    public string Title { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public int MaxUsage { get; set; }
    public int Percent { get; set; }
    public double MaxUnit { get; set; }
    public Guid? ForUser { get; set; }
    public Guid? PlanId { get; set; }
    public CostUnit Unit { get; set; }
    public int Index { get; set; }
}