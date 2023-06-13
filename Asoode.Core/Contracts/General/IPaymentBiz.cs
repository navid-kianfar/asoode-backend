using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Payment.PayPing;

namespace Asoode.Core.Contracts.General;

public interface IPaymentBiz
{
    Task<OperationResult<string>> PayByPayPing(Guid orderId);
    Task<OperationResult<ResponseVerifyViewModel>> VerifyPayPing(double amount, string transId);
    Task<OperationResult<string>> ConfirmOrder(object unitObj, object orderObj);
}