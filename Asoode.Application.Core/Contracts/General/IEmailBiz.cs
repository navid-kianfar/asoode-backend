using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Payment;

namespace Asoode.Application.Core.Contracts.General;

public interface IEmailBiz
{
    Task<OperationResult<bool>> ChangeEmail(string subject, string userId, string email, string token);

    Task<OperationResult<bool>> Forget(string subject, string userId, string email, string token);

    Task<string> LoadTemplate(string template);

    Task<OperationResult<bool>> Register(string subject, string userId, string email, string token);

    Task<OperationResult<bool>> Send(string subject, string to, string body, bool isHtml = true);

    Task InviteGroup(string subject, string fullName, string[] noneMembers, Dictionary<string, string> members,
        Guid groupId, string groupTitle);

    Task<OperationResult<bool>> EmailWelcome(string subject, string userId, string email);

    Task InviteProject(string subject, string fullName, string[] noneMembers, Dictionary<string, string> members,
        string section, string id, string projectName);

    Task<OperationResult<bool>> OrderCreated(string subject, string planTitle, MemberInfoViewModel user,
        OrderViewModel order);

    Task<OperationResult<bool>> OrderPaid(string subject, MemberInfoViewModel user, OrderViewModel order);
    Task<OperationResult<bool>> Reply(string subject, string email, string message);
}