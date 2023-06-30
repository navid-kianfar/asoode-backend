using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class ProjectService : IProjectService
{
    public Task<OperationResult<ProjectDto[]>> List(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ProjectDto[]>> Archived(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ProjectPrepareDto>> Import(Guid userId, ImportDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Create(Guid userId, ProjectCreateDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId, ChangeAccessDto permission)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> AddAccess(Guid userId, Guid projectId, AccessDto invite)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Remove(Guid userId, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Archive(Guid userId, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Export(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CreateSubProject(Guid userId, Guid projectId, CreateSubProjectDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid accessId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid accessId, ChangeAccessDto permission)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditSubProject(Guid userId, Guid subId, SimpleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveSubProject(Guid userId, Guid subId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CreateSeason(Guid userId, Guid projectId, SimpleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditSeason(Guid userId, Guid seasonId, SimpleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveSeason(Guid userId, Guid seasonId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditProject(Guid identityUserId, Guid id, ProjectEditDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> OrderSubProject(Guid userId, Guid id, ChangeOrderDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<WorkPackageObjectiveDto[]>> Objectives(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ProjectObjectiveEstimatedPriceDto[]>> ObjectiveDetails(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<TreeDto>> Tree(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ProjectProgressDto[]>> ProjectProgress(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<RoadMapDto>> RoadMap(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ProjectDto>> Fetch(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}