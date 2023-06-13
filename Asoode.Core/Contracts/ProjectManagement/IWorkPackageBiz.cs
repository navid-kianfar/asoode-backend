using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.ProjectManagement;

namespace Asoode.Core.Contracts.ProjectManagement;

public interface IWorkPackageBiz
{
    Task<OperationResult<WorkPackageViewModel>> Fetch(Guid userId, Guid packageId,
        WorkPackageFilterViewModel filter);

    Task<OperationResult<bool>> CreateWorkPackage(Guid userId, Guid projectId, CreateWorkPackageViewModel model);
    Task<OperationResult<bool>> AddAccess(Guid userId, Guid workPackageId, AccessViewModel permission);
    Task<OperationResult<bool>> CreateList(Guid userId, Guid workPackageId, TitleViewModel model);
    Task<OperationResult<bool>> RepositionList(Guid userId, Guid listId, RepositionViewModel model);
    Task<OperationResult<bool>> RenameList(Guid userId, Guid listId, TitleViewModel model);
    Task<OperationResult<bool>> CreateObjective(Guid userId, Guid workPackageId, CreateObjectiveViewModel model);
    Task<OperationResult<bool>> EditObjective(Guid userId, Guid objectiveId, CreateObjectiveViewModel model);
    Task<OperationResult<bool>> DeleteObjective(Guid userId, Guid objectiveId);
    Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid accessId);
    Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId, ChangeAccessViewModel permission);
    Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid id);
    Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid id, ChangeAccessViewModel permission);
    Task<OperationResult<bool>> CreateLabel(Guid userId, Guid packageId, LabelViewModel model);
    Task<OperationResult<bool>> RenameLabel(Guid userId, Guid labelId, LabelViewModel model);
    Task<OperationResult<bool>> RemoveLabel(Guid userId, Guid labelId);
    Task<OperationResult<bool>> Setting(Guid userId, Guid id, WorkPackageSettingViewModel model);
    Task<OperationResult<bool>> UserSetting(Guid userId, Guid id, WorkPackageUserSettingViewModel model);
    Task<OperationResult<bool>> Leave(Guid userId, Guid id);
    Task<OperationResult<bool>> Archive(Guid userId, Guid id);
    Task<OperationResult<bool>> Order(Guid userId, Guid id, ChangeOrderViewModel model);
    Task<OperationResult<bool>> Edit(Guid userId, Guid id, SimpleViewModel model);
    Task<OperationResult<bool>> Merge(Guid userId, Guid id, Guid destinationId);
    Task<OperationResult<bool>> Connect(Guid userId, Guid id, Guid projectId);
    Task<OperationResult<bool>> Upgrade(Guid userId, Guid id);
    Task<OperationResult<bool>> CloneList(Guid userId, Guid id, TitleViewModel model);
    Task<OperationResult<bool>> ArchiveList(Guid userId, Guid id);
    Task<OperationResult<bool>> ArchiveListTasks(Guid userId, Guid id);
    Task<OperationResult<bool>> Permissions(Guid userId, Guid packageId, WorkPackagePermissionViewModel permission);
    Task<OperationResult<bool>> Remove(Guid userId, Guid packageId);

    Task<OperationResult<WorkPackageViewModel>> FetchArchived(Guid userId, Guid packageId,
        WorkPackageFilterViewModel filter);

    Task<OperationResult<bool>> SortOrder(Guid userId, Guid packageId, SortOrderViewModel model);
    Task<OperationResult<bool>> DeleteListTasks(Guid userId, Guid id);
}