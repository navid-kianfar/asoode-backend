using Asoode.Shared.Abstraction.Dtos.Membership.Order;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface IOrderService
{
    Task<OperationResult<OrderDiscountResultDto>> CheckDiscount(Guid userId, CheckDiscountDto model);
    Task<OperationResult<Guid>> Order(Guid userId, RequestOrderDto model);
    Task<OperationResult<string>> Pay(Guid orderId);
    Task<OperationResult<Tuple<bool, string>>> PayPingCallBack(string transId, Guid paymentId);
    Task<Stream?> Pdf(Guid id);
}