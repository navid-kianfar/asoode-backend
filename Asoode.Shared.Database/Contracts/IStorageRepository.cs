using Asoode.Shared.Abstraction.Dtos.Communication;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Dtos.User;

namespace Asoode.Shared.Database.Contracts;

public interface IStorageRepository
{
    Task<UploadDto[]> GetDirectories(string key, string directory);
    Task<UploadDto[]> GetFiles(string directory);
    Task<UserDto?> FindUser(Guid userId);
    Task<ProjectDto[]> FindProjects(Guid userId);
    Task<WorkPackageDto[]> FindWorkPackages(Guid userId, Guid projectId);
    Task<WorkPackageTaskDto[]> FindProjectTasks(Guid userId, Guid projectId);
    Task<WorkPackageTaskDto[]> FindPackageTasks(Guid userId, Guid projectId);
    Task<ChannelDto[]> FindChannels(Guid userId);
    Task<WorkPackageTaskAttachmentDto[]> GetTaskAttachments(Guid taskId, Guid userId, bool byUser);
    Task<UploadDto[]> GetChannelAttachments(Guid channelId, Guid userId, bool byUser);
    Task<bool> NewFolder(Guid userId, string directory, string path);
    Task<UserPlanInfoDto?> GetUserPlan(Guid userId);
    Task<bool> Store(UploadDto dto);
}