namespace Asoode.Application.Core.ViewModels.Membership.Order;

public class OrderDiscountResultViewModel
{
    public bool Success { get; set; }
    public bool Invalid { get; set; }
    public bool Expired { get; set; }
    public bool AlreadyUsed { get; set; }
    public double Amount { get; set; }
    public Guid? Id { get; set; }
    public bool InvalidPlan { get; set; }
}