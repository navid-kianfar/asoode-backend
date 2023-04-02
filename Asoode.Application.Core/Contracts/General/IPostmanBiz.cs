using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Payment;
using Asoode.Application.Core.ViewModels.ProjectManagement;

namespace Asoode.Application.Core.Contracts.General;

public interface IPostmanBiz
{
    Task InviteGroup(string fullName, string[] noneMembers, Dictionary<string, string> members,
        Guid groupId, string groupTitle);

    Task<OperationResult<bool>> EmailWelcome(string userId, string email);

    Task InviteProject(string fullName, string[] emailIdentities, Dictionary<string, string> mapped,
        ProjectViewModel payload);

    Task InviteWorkPackage(string fullName, string[] emailIdentities, Dictionary<string, string> mapped,
        WorkPackageViewModel payload);
}