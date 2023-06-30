using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Dtos.User;

namespace Asoode.Shared.Database.Contracts;

public interface ISearchRepository
{
    Task<MemberInfoDto[]> GetAllUsers(Guid[] everyOne, string search);
    Task<CombinedGroupMemberDto[]> GetAllGroups(Guid userId);
    Task<CombinedProjectMemberDto[]> GetAllProjects(Guid userId, Guid[] groupIds);
    Task<CombinedWorkPackageMemberDto[]> GetAllWorkPackages(Guid userId, Guid[] allGroupIds, Guid[] allProjectIds);
    Task<CombinedTaskListNameDto[]> GetAllTasks(Guid[] allPackageIds, string search);
    Task<UploadDto[]> GetAllUploads(Guid userId, string search);
    Task<CombinedTaskMemberUserDto[]> GetAllTaskMembers(Guid[] allTaskIds);
    Task<CombinedLabelTaskLabelDto[]> GetAllLabeledTasks(Guid[] allTaskIds);
}