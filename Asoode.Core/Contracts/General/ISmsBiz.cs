using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Payment;

namespace Asoode.Core.Contracts.General;

public interface ISmsBiz
{
    Task<string> ChangeNumber(string token);

    Task<string> Forget(string tokenCode);

    Task<string> LoadTemplate(string template);

    Task<string> Register(string code);

    Task<OperationResult<bool>> Send(string mobile, string message);
    Task<OperationResult<bool>> OrderCreated(string planTitle, MemberInfoViewModel user, OrderViewModel order);
    Task<OperationResult<bool>> OrderPaid(MemberInfoViewModel user, OrderViewModel order);
}