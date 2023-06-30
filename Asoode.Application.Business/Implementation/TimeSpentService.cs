using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Application.Business.Implementation;

internal class TimeSpentService : ITimeSpentService
{
    private readonly ITimeSpentRepository _repository;

    public TimeSpentService(ITimeSpentRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResult<TimeSpentDto[]>> TimeSpents(Guid userId, DurationDto model)
    {
        var user = await _repository.GetUser(userId);
        if (user == null || user.IsLocked || user.Blocked)
            return OperationResult<TimeSpentDto[]>.Rejected();

        var result = await _repository.UserTimeSpents(userId, model.Begin, model.End);
        return OperationResult<TimeSpentDto[]>.Success(result);
    }

    public async Task<OperationResult<TimeSpentDto[]>> GroupTimeSpents(Guid userId, Guid groupId, DurationDto model)
    {
        var user = await _repository.GetUser(userId);
        if (user == null || user.IsLocked || user.Blocked)
            return OperationResult<TimeSpentDto[]>.Rejected();

        var access = await _repository.GroupAccess(userId, groupId);
        if (access == null) return OperationResult<TimeSpentDto[]>.Rejected();

        var packages = await _repository.UserPackages(groupId);
        
        var result = await _repository.GroupTimeSpent(userId, groupId, access, packages, model.Begin, model.End);
        return OperationResult<TimeSpentDto[]>.Success(result);
    }
}