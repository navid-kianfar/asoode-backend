using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Payment;
using Asoode.Application.Core.ViewModels.ProjectManagement;

namespace Asoode.Application.Core.Contracts.General;

public interface IPostmanBiz
{
    Task<OperationResult<bool>> EmailChange(string userId, string email, string token);

    Task<OperationResult<bool>> EmailConfirmAccount(string userId, string email, string token);

    Task<OperationResult<bool>> EmailForgetPassword(string userId, string email, string token);
    Task<OperationResult<bool>> PhoneChange(string phone, string token);

    Task<OperationResult<bool>> PhoneConfirmAccount(string phone, string token);

    Task<OperationResult<bool>> PhoneForgetPassword(string phone, string token);

    Task InviteGroup(string fullName, string[] noneMembers, Dictionary<string, string> members,
        Guid groupId, string groupTitle);

    Task<OperationResult<bool>> EmailWelcome(string userId, string email);

    Task InviteProject(string fullName, string[] emailIdentities, Dictionary<string, string> mapped,
        ProjectViewModel payload);

    Task InviteWorkPackage(string fullName, string[] emailIdentities, Dictionary<string, string> mapped,
        WorkPackageViewModel payload);

    Task OrderCreated(string planTitle, MemberInfoViewModel user, OrderViewModel order);

    Task OrderPaid(MemberInfoViewModel user, OrderViewModel order);
    Task<OperationResult<bool>> Reply(string email, string message);
}