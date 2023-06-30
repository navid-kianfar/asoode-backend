namespace Asoode.Shared.Abstraction.Dtos.Membership.Order;

public record OrderDiscountResultDto
{
    public bool Success { get; set; }
    public bool Invalid { get; set; }
    public bool Expired { get; set; }
    public bool AlreadyUsed { get; set; }
    public double Amount { get; set; }
    public Guid? Id { get; set; }
    public bool InvalidPlan { get; set; }
}