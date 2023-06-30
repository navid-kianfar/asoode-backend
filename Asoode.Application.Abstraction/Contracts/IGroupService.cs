using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface IGroupService
{
    Task<OperationResult<GroupDto[]>> List(Guid userId);
    Task<OperationResult<GroupDto[]>> Archived(Guid userId);
    Task<OperationResult<bool>> Create(Guid userId, GroupCreateDto model);
    Task<OperationResult<bool>> Edit(Guid userId, Guid groupId, GroupDto model);
    Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid id);
    Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId, ChangeAccessDto permission);
    Task<OperationResult<bool>> AddAccess(Guid userId, Guid groupId, AccessDto invite);
    Task<OperationResult<bool>> Remove(Guid userId, Guid groupId);
    Task<OperationResult<bool>> Export(Guid userId, Guid id);
    Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid accessId);
    Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid accessId, ChangeAccessDto permission);
    Task<OperationResult<DashBoardProgressDto[]>> Report(Guid userId, Guid id, DurationDto model);
    Task<OperationResult<GridResult<ShiftDto>>> Shifts(Guid userId, Guid id, GridFilter filter);
    Task<OperationResult<GridResult<TimeOffDto>>> TimeOffs(Guid userId, Guid id, GridFilter filter);
    Task<OperationResult<bool>> Archive(Guid userId, Guid id);
    Task<OperationResult<GroupDto>> Fetch(Guid userId, Guid id);
    Task<OperationResult<bool>> Restore(Guid userId, Guid id);
    Task<OperationResult<GridResult<EntryLogDto>>> EntryLogs(Guid userId, Guid id, GridFilter filter);
    Task<OperationResult<bool>> ToggleEntry(Guid userId, Guid id);
    Task<OperationResult<bool>> RemoveEntry(Guid userId, Guid id);
    Task<OperationResult<bool>> EditEntry(Guid userId, Guid id, OptionalDurationDto model);
    Task<OperationResult<bool>> CreateShift(Guid userId, Guid groupId, EditShiftDto model);
    Task<OperationResult<bool>> EditShift(Guid userId, Guid shiftId, EditShiftDto model);
    Task<OperationResult<bool>> DeleteShift(Guid userId, Guid shiftId);
    Task<OperationResult<bool>> ManualEntry(Guid userId, Guid groupId, ManualEntryDto model);
    Task<OperationResult<bool>> Upgrade(Guid userId, Guid id);
    Task<OperationResult<bool>> Connect(Guid userId, Guid parentId, Guid id);
    Task<OperationResult<SelectableItem<Guid>[]>> NonAttached(Guid userId, Guid id);
    Task<OperationResult<bool>> RequestTimeOff(Guid userId, Guid id, RequestTimeOffDto model);
    Task<OperationResult<bool>> TimeOffResponse(Guid userId, Guid id, bool approve);
    Task<OperationResult<TimeOffDetailDto>> TimeOffDetail(Guid userId, Guid id);
    Task<OperationResult<bool>> RemoveTimeOff(Guid userId, Guid id);

    Task<OperationResult<GridResult<TimeOffDto>>> TimeOffHistory(Guid userId, Guid id,
        GridFilterWithParams<IdDto> filter);
}