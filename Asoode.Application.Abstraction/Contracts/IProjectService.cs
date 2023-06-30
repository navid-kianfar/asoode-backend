using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface IProjectService
{
    Task<OperationResult<ProjectDto[]>> List(Guid userId);
    Task<OperationResult<ProjectDto[]>> Archived(Guid userId);
    Task<OperationResult<ProjectPrepareDto>> Import(Guid userId, ImportDto model);
    Task<OperationResult<bool>> Create(Guid userId, ProjectCreateDto model);
    Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid id);
    Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId, ChangeAccessDto permission);
    Task<OperationResult<bool>> AddAccess(Guid userId, Guid projectId, AccessDto invite);
    Task<OperationResult<bool>> Remove(Guid userId, Guid projectId);
    Task<OperationResult<bool>> Archive(Guid userId, Guid projectId);
    Task<OperationResult<bool>> Export(Guid userId, Guid id);
    Task<OperationResult<bool>> CreateSubProject(Guid userId, Guid projectId, CreateSubProjectDto model);
    Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid accessId);
    Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid accessId, ChangeAccessDto permission);
    Task<OperationResult<bool>> EditSubProject(Guid userId, Guid subId, SimpleDto model);
    Task<OperationResult<bool>> RemoveSubProject(Guid userId, Guid subId);
    Task<OperationResult<bool>> CreateSeason(Guid userId, Guid projectId, SimpleDto model);
    Task<OperationResult<bool>> EditSeason(Guid userId, Guid seasonId, SimpleDto model);
    Task<OperationResult<bool>> RemoveSeason(Guid userId, Guid seasonId);
    Task<OperationResult<bool>> EditProject(Guid identityUserId, Guid id, ProjectEditDto model);
    Task<OperationResult<bool>> OrderSubProject(Guid userId, Guid id, ChangeOrderDto model);
    Task<OperationResult<WorkPackageObjectiveDto[]>> Objectives(Guid userId, Guid id);
    Task<OperationResult<ProjectObjectiveEstimatedPriceDto[]>> ObjectiveDetails(Guid userId, Guid id);
    Task<OperationResult<TreeDto>> Tree(Guid userId, Guid id);
    Task<OperationResult<ProjectProgressDto[]>> ProjectProgress(Guid userId, Guid id);
    Task<OperationResult<RoadMapDto>> RoadMap(Guid userId, Guid id);
    Task<OperationResult<ProjectDto>> Fetch(Guid userId, Guid id);
}