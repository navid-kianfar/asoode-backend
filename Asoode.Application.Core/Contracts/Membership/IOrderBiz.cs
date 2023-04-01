using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.Membership.Order;

namespace Asoode.Application.Core.Contracts.Membership;

public interface IOrderBiz
{
    Task<OperationResult<OrderDiscountResultViewModel>> CheckDiscount(Guid userId, CheckDiscountViewModel model);
    Task<OperationResult<Guid>> Order(Guid userId, RequestOrderViewModel model);
    Task<OperationResult<string>> Pay(Guid orderId);
    Task<OperationResult<Tuple<bool, string>>> PayPingCallBack(string transId, Guid paymentId);
    Task<Stream> Pdf(Guid id);
}