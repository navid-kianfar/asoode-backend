using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface ITaskService
{
    Task<OperationResult<bool>> Create(Guid userId, Guid packageId, CreateTaskDto model);
    Task<OperationResult<bool>> Reposition(Guid userId, Guid taskId, RepositionDto model);
    Task<OperationResult<bool>> Move(Guid userId, Guid taskId, MoveTaskDto model);
    Task<OperationResult<WorkPackageTaskDto>> Detail(Guid userId, Guid taskId);
    Task<OperationResult<bool>> Comment(Guid userId, Guid taskId, PostTaskCommentDto model);
    Task<OperationResult<bool>> ChangeTitle(Guid userId, Guid id, TitleDto model);
    Task<OperationResult<bool>> ChangeDescription(Guid userId, Guid id, TitleDto model);
    Task<OperationResult<bool>> ChangeState(Guid userId, Guid id, StateDto model);
    Task<OperationResult<bool>> AddMember(Guid userId, Guid id, TaskMemberDto model);
    Task<OperationResult<bool>> RemoveMember(Guid userId, Guid taskId, Guid recordId);
    Task<OperationResult<bool>> AddLabel(Guid userId, Guid taskId, Guid labelId);
    Task<OperationResult<bool>> RemoveLabel(Guid userId, Guid taskId, Guid labelId);
    Task<OperationResult<UploadResultDto>> AddAttachment(Guid userId, Guid taskId, StorageItemDto file);
    Task<OperationResult<bool>> RemoveAttachment(Guid userId, Guid attachmentId);
    Task<OperationResult<bool>> RenameAttachment(Guid userId, Guid attachmentId, TitleDto model);
    Task<OperationResult<bool>> Archive(Guid userId, Guid taskId);
    Task<OperationResult<bool>> Watch(Guid userId, Guid taskId);
    Task<OperationResult<bool>> CoverAttachment(Guid userId, Guid attachmentId);
    Task<OperationResult<bool>> Location(Guid userId, Guid taskId, LocationDto model);
    Task<OperationResult<TaskLogDto[]>> Logs(Guid userId, Guid taskId);
    Task<OperationResult<bool>> Estimated(Guid userId, Guid taskId, EstimatedTimeDto model);
    Task<OperationResult<bool>> SpendTime(Guid userId, Guid taskId, DurationDto model);
    Task<OperationResult<bool>> Vote(Guid userId, Guid taskId, VoteDto model);
    Task<OperationResult<bool>> VoteSetting(Guid userId, Guid taskId, VoteSettingDto model);
    Task<OperationResult<bool>> VoteClear(Guid userId, Guid taskId);
    Task<OperationResult<bool>> SetDate(Guid userId, Guid taskId, SetDateDto model);
    Task<OperationResult<WorkPackageTaskDto[]>> Calendar(Guid userId, DurationDto model);
    Task<OperationResult<KartablDto>> Kartabl(Guid userId, DurationDto model);

    Task<OperationResult<AdvancedPlayerDto>> FetchAdvanced(Guid userId, Guid attachmentId);

    Task<OperationResult<AdvancedPlayerCommentDto>> CommentAdvanced(Guid userId, Guid attachmentId,
        EditAdvancedCommentDto model);

    Task<OperationResult<bool>> EditAdvancedComment(Guid userId, Guid commentId, TitleDto model);
    Task<OperationResult<bool>> RemoveAdvancedComment(Guid userId, Guid commentId);
    Task<OperationResult<PdfAdvancedCommentDto>> PdfAdvanced(Guid userId, Guid attachmentId);
    Task<OperationResult<UploadResultDto>> BulkAttachment(Guid userId, Guid id, StorageItemDto file);
    Task<OperationResult<BulkDownloadResultDto>> BulkDownload(Guid userId, Guid id, Guid[] picked);
}