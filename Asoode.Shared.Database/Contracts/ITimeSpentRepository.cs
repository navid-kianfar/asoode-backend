using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.User;

namespace Asoode.Shared.Database.Contracts;

public interface ITimeSpentRepository
{
    Task<TimeSpentDto[]> UserTimeSpents(Guid userId, DateTime begin, DateTime end);
    Task<UserDto?> GetUser(Guid userId);
    Task<GroupMemberDto?> GroupAccess(Guid userId, Guid groupId);
    Task<Guid[]> UserPackages(Guid groupId);
    Task<TimeSpentDto[]> GroupTimeSpent(Guid userId, Guid groupId, GroupMemberDto access, Guid[] packages, DateTime begin, DateTime end);
}