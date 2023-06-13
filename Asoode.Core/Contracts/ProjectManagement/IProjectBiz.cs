using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.ProjectManagement;

namespace Asoode.Core.Contracts.ProjectManagement;

public interface IProjectBiz
{
    Task<OperationResult<ProjectViewModel[]>> List(Guid userId);
    Task<OperationResult<ProjectViewModel[]>> Archived(Guid userId);
    Task<OperationResult<ProjectPrepareViewModel>> Import(Guid userId, ImportViewModel model);
    Task<OperationResult<bool>> Create(Guid userId, ProjectCreateViewModel model);
    Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid id);
    Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId, ChangeAccessViewModel permission);
    Task<OperationResult<bool>> AddAccess(Guid userId, Guid projectId, AccessViewModel invite);
    Task<OperationResult<bool>> Remove(Guid userId, Guid projectId);
    Task<OperationResult<bool>> Archive(Guid userId, Guid projectId);
    Task<OperationResult<bool>> Export(Guid userId, Guid id);
    Task<OperationResult<bool>> CreateSubProject(Guid userId, Guid projectId, CreateSubProjectViewModel model);
    Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid accessId);
    Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid accessId, ChangeAccessViewModel permission);
    Task<OperationResult<bool>> EditSubProject(Guid userId, Guid subId, SimpleViewModel model);
    Task<OperationResult<bool>> RemoveSubProject(Guid userId, Guid subId);
    Task<OperationResult<bool>> CreateSeason(Guid userId, Guid projectId, SimpleViewModel model);
    Task<OperationResult<bool>> EditSeason(Guid userId, Guid seasonId, SimpleViewModel model);
    Task<OperationResult<bool>> RemoveSeason(Guid userId, Guid seasonId);
    Task<OperationResult<bool>> EditProject(Guid identityUserId, Guid id, ProjectEditViewModel model);
    Task<OperationResult<bool>> OrderSubProject(Guid userId, Guid id, ChangeOrderViewModel model);
    Task<OperationResult<WorkPackageObjectiveViewModel[]>> Objectives(Guid userId, Guid id);
    Task<OperationResult<ProjectObjectiveEstimatedPriceViewModel[]>> ObjectiveDetails(Guid userId, Guid id);
    Task<OperationResult<TreeViewModel>> Tree(Guid userId, Guid id);
    Task<OperationResult<ProjectProgressViewModel[]>> ProjectProgress(Guid userId, Guid id);
    Task<OperationResult<RoadMapViewModel>> RoadMap(Guid userId, Guid id);
    Task<OperationResult<ProjectViewModel>> Fetch(Guid userId, Guid id);
}