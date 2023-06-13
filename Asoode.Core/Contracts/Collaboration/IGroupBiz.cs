using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Collaboration;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Reports;

namespace Asoode.Core.Contracts.Collaboration;

public interface IGroupBiz
{
    Task<OperationResult<GroupViewModel[]>> List(Guid userId);
    Task<OperationResult<GroupViewModel[]>> Archived(Guid userId);
    Task<OperationResult<bool>> Create(Guid userId, GroupCreateViewModel model);
    Task<OperationResult<bool>> Edit(Guid userId, Guid groupId, GroupViewModel model);
    Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid id);
    Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId, ChangeAccessViewModel permission);
    Task<OperationResult<bool>> AddAccess(Guid userId, Guid groupId, AccessViewModel invite);
    Task<OperationResult<bool>> Remove(Guid userId, Guid groupId);
    Task<OperationResult<bool>> Export(Guid userId, Guid id);
    Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid accessId);
    Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid accessId, ChangeAccessViewModel permission);
    Task<OperationResult<DashBoardProgressViewModel[]>> Report(Guid userId, Guid id, DurationViewModel model);
    Task<OperationResult<GridResult<ShiftViewModel>>> Shifts(Guid userId, Guid id, GridFilter filter);
    Task<OperationResult<GridResult<TimeOffViewModel>>> TimeOffs(Guid userId, Guid id, GridFilter filter);
    Task<OperationResult<bool>> Archive(Guid userId, Guid id);
    Task<OperationResult<GroupViewModel>> Fetch(Guid userId, Guid id);
    Task<OperationResult<bool>> Restore(Guid userId, Guid id);
    Task<OperationResult<GridResult<EntryLogViewModel>>> EntryLogs(Guid userId, Guid id, GridFilter filter);
    Task<OperationResult<bool>> ToggleEntry(Guid userId, Guid id);
    Task<OperationResult<bool>> RemoveEntry(Guid userId, Guid id);
    Task<OperationResult<bool>> EditEntry(Guid userId, Guid id, OptionalDurationViewModel model);
    Task<OperationResult<bool>> CreateShift(Guid userId, Guid groupId, EditShiftViewModel model);
    Task<OperationResult<bool>> EditShift(Guid userId, Guid shiftId, EditShiftViewModel model);
    Task<OperationResult<bool>> DeleteShift(Guid userId, Guid shiftId);
    Task<OperationResult<bool>> ManualEntry(Guid userId, Guid groupId, ManualEntryViewModel model);
    Task<OperationResult<bool>> Upgrade(Guid userId, Guid id);
    Task<OperationResult<bool>> Connect(Guid userId, Guid parentId, Guid id);
    Task<OperationResult<SelectableItem<Guid>[]>> NonAttached(Guid userId, Guid id);
    Task<OperationResult<bool>> RequestTimeOff(Guid userId, Guid id, RequestTimeOffViewModel model);
    Task<OperationResult<bool>> TimeOffResponse(Guid userId, Guid id, bool approve);
    Task<OperationResult<TimeOffDetailViewModel>> TimeOffDetail(Guid userId, Guid id);
    Task<OperationResult<bool>> RemoveTimeOff(Guid userId, Guid id);

    Task<OperationResult<GridResult<TimeOffViewModel>>> TimeOffHistory(Guid userId, Guid id,
        GridFilterWithParams<IdViewModel> filter);
}