using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class WorkPackageService : IWorkPackageService
{
    public Task<OperationResult<WorkPackageDto>> Fetch(Guid userId, Guid packageId, WorkPackageFilterDto filter)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CreateWorkPackage(Guid userId, Guid projectId, CreateWorkPackageDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> AddAccess(Guid userId, Guid workPackageId, AccessDto permission)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CreateList(Guid userId, Guid workPackageId, TitleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RepositionList(Guid userId, Guid listId, RepositionDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RenameList(Guid userId, Guid listId, TitleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CreateObjective(Guid userId, Guid workPackageId, CreateObjectiveDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditObjective(Guid userId, Guid objectiveId, CreateObjectiveDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> DeleteObjective(Guid userId, Guid objectiveId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid accessId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId, ChangeAccessDto permission)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid id, ChangeAccessDto permission)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CreateLabel(Guid userId, Guid packageId, LabelDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RenameLabel(Guid userId, Guid labelId, LabelDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveLabel(Guid userId, Guid labelId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Setting(Guid userId, Guid id, WorkPackageSettingDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> UserSetting(Guid userId, Guid id, WorkPackageUserSettingDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Leave(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Archive(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Order(Guid userId, Guid id, ChangeOrderDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Edit(Guid userId, Guid id, SimpleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Merge(Guid userId, Guid id, Guid destinationId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Connect(Guid userId, Guid id, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Upgrade(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CloneList(Guid userId, Guid id, TitleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ArchiveList(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ArchiveListTasks(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Permissions(Guid userId, Guid packageId, WorkPackagePermissionDto permission)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Remove(Guid userId, Guid packageId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<WorkPackageDto>> FetchArchived(Guid userId, Guid packageId, WorkPackageFilterDto filter)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> SortOrder(Guid userId, Guid packageId, SortOrderDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> DeleteListTasks(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}