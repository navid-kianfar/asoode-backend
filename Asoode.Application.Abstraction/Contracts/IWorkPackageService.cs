using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface IWorkPackageService
{
    Task<OperationResult<WorkPackageDto>> Fetch(Guid userId, Guid packageId,
        WorkPackageFilterDto filter);

    Task<OperationResult<bool>> CreateWorkPackage(Guid userId, Guid projectId, CreateWorkPackageDto model);
    Task<OperationResult<bool>> AddAccess(Guid userId, Guid workPackageId, AccessDto permission);
    Task<OperationResult<bool>> CreateList(Guid userId, Guid workPackageId, TitleDto model);
    Task<OperationResult<bool>> RepositionList(Guid userId, Guid listId, RepositionDto model);
    Task<OperationResult<bool>> RenameList(Guid userId, Guid listId, TitleDto model);
    Task<OperationResult<bool>> CreateObjective(Guid userId, Guid workPackageId, CreateObjectiveDto model);
    Task<OperationResult<bool>> EditObjective(Guid userId, Guid objectiveId, CreateObjectiveDto model);
    Task<OperationResult<bool>> DeleteObjective(Guid userId, Guid objectiveId);
    Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid accessId);
    Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId, ChangeAccessDto permission);
    Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid id);
    Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid id, ChangeAccessDto permission);
    Task<OperationResult<bool>> CreateLabel(Guid userId, Guid packageId, LabelDto model);
    Task<OperationResult<bool>> RenameLabel(Guid userId, Guid labelId, LabelDto model);
    Task<OperationResult<bool>> RemoveLabel(Guid userId, Guid labelId);
    Task<OperationResult<bool>> Setting(Guid userId, Guid id, WorkPackageSettingDto model);
    Task<OperationResult<bool>> UserSetting(Guid userId, Guid id, WorkPackageUserSettingDto model);
    Task<OperationResult<bool>> Leave(Guid userId, Guid id);
    Task<OperationResult<bool>> Archive(Guid userId, Guid id);
    Task<OperationResult<bool>> Order(Guid userId, Guid id, ChangeOrderDto model);
    Task<OperationResult<bool>> Edit(Guid userId, Guid id, SimpleDto model);
    Task<OperationResult<bool>> Merge(Guid userId, Guid id, Guid destinationId);
    Task<OperationResult<bool>> Connect(Guid userId, Guid id, Guid projectId);
    Task<OperationResult<bool>> Upgrade(Guid userId, Guid id);
    Task<OperationResult<bool>> CloneList(Guid userId, Guid id, TitleDto model);
    Task<OperationResult<bool>> ArchiveList(Guid userId, Guid id);
    Task<OperationResult<bool>> ArchiveListTasks(Guid userId, Guid id);
    Task<OperationResult<bool>> Permissions(Guid userId, Guid packageId, WorkPackagePermissionDto permission);
    Task<OperationResult<bool>> Remove(Guid userId, Guid packageId);

    Task<OperationResult<WorkPackageDto>> FetchArchived(Guid userId, Guid packageId,
        WorkPackageFilterDto filter);

    Task<OperationResult<bool>> SortOrder(Guid userId, Guid packageId, SortOrderDto model);
    Task<OperationResult<bool>> DeleteListTasks(Guid userId, Guid id);
}