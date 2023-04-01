using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.Payment.PayPing;

namespace Asoode.Application.Core.Contracts.General;

public interface IPaymentBiz
{
    Task<OperationResult<string>> PayByPayPing(Guid orderId);
    Task<OperationResult<ResponseVerifyViewModel>> VerifyPayPing(double amount, string transId);
    Task<OperationResult<string>> ConfirmOrder(object unitObj, object orderObj);
}