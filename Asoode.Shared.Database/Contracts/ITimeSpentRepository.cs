using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.User;

namespace Asoode.Shared.Database.Contracts;

public interface ITimeSpentRepository
{
    Task<TimeSpentDto[]> UserTimeSpents(Guid userId);
    Task<UserDto?> GetUser(Guid userId);
    Task<AccessDto?> GroupAccess(Guid userId, Guid groupId);
    Task<Guid[]> UserPackages(Guid groupId);
    Task<TimeSpentDto[]> GroupTimeSpent(Guid groupId, AccessDto access, Guid[] packages);
}