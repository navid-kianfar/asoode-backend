using Asoode.Shared.Abstraction.Dtos.Discount;
using Asoode.Shared.Abstraction.Dtos.Payment;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Dtos.User;

namespace Asoode.Shared.Database.Contracts;

public interface IOrderRepository
{
    Task<UserDto?> GetUser(Guid userId);
    Task<DiscountDto?> GetDiscount(string code);
    Task<int> DidUserAlreadyUsedDiscount(Guid userId, Guid discountId);
    Task<PlanDto[]> PlansList(Guid planId);
    Task Store(OrderDto order);
    Task<UserPlanInfoDto?> GetUserPlan(Guid userId);
    Task CancelAnyPendingOrder(Guid userId);
}