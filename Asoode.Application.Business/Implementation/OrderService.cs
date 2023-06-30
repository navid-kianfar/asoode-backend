using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Membership.Order;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class OrderService : IOrderService
{
    public Task<OperationResult<OrderDiscountResultDto>> CheckDiscount(Guid userId, CheckDiscountDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<Guid>> Order(Guid userId, RequestOrderDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<string>> Pay(Guid orderId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<Tuple<bool, string>>> PayPingCallBack(string transId, Guid paymentId)
    {
        throw new NotImplementedException();
    }

    public Task<Stream?> Pdf(Guid id)
    {
        throw new NotImplementedException();
    }
}