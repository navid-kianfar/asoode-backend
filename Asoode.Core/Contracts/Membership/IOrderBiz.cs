using System;
using System.IO;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Membership.Order;

namespace Asoode.Core.Contracts.Membership;

public interface IOrderBiz
{
    Task<OperationResult<OrderDiscountResultViewModel>> CheckDiscount(Guid userId, CheckDiscountViewModel model);
    Task<OperationResult<Guid>> Order(Guid userId, RequestOrderViewModel model);
    Task<OperationResult<string>> Pay(Guid orderId);
    Task<OperationResult<Tuple<bool, string>>> PayPingCallBack(string transId, Guid paymentId);
    Task<Stream> Pdf(Guid id);
}