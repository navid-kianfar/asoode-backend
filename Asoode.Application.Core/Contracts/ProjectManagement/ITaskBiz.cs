using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Reports;
using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Core.Contracts.ProjectManagement;

public interface ITaskBiz
{
    Task<OperationResult<bool>> Create(Guid userId, Guid packageId, CreateTaskViewModel model);
    Task<OperationResult<bool>> Reposition(Guid userId, Guid taskId, RepositionViewModel model);
    Task<OperationResult<bool>> Move(Guid userId, Guid taskId, MoveTaskViewModel model);
    Task<OperationResult<WorkPackageTaskViewModel>> Detail(Guid userId, Guid taskId);
    Task<OperationResult<bool>> Comment(Guid userId, Guid taskId, PostTaskCommentViewModel model);
    Task<OperationResult<bool>> ChangeTitle(Guid userId, Guid id, TitleViewModel model);
    Task<OperationResult<bool>> ChangeDescription(Guid userId, Guid id, TitleViewModel model);
    Task<OperationResult<bool>> ChangeState(Guid userId, Guid id, StateViewModel model);
    Task<OperationResult<bool>> AddMember(Guid userId, Guid id, TaskMemberViewModel model);
    Task<OperationResult<bool>> RemoveMember(Guid userId, Guid taskId, Guid recordId);
    Task<OperationResult<bool>> AddLabel(Guid userId, Guid taskId, Guid labelId);
    Task<OperationResult<bool>> RemoveLabel(Guid userId, Guid taskId, Guid labelId);
    Task<OperationResult<UploadResultViewModel>> AddAttachment(Guid userId, Guid taskId, UploadedFileViewModel file);
    Task<OperationResult<bool>> RemoveAttachment(Guid userId, Guid attachmentId);
    Task<OperationResult<bool>> RenameAttachment(Guid userId, Guid attachmentId, TitleViewModel model);
    Task<OperationResult<bool>> Archive(Guid userId, Guid taskId);
    Task<OperationResult<bool>> Watch(Guid userId, Guid taskId);
    Task<OperationResult<bool>> CoverAttachment(Guid userId, Guid attachmentId);
    Task<OperationResult<bool>> Location(Guid userId, Guid taskId, LocationViewModel model);
    Task<OperationResult<TaskLogViewModel[]>> Logs(Guid userId, Guid taskId);
    Task<OperationResult<bool>> Estimated(Guid userId, Guid taskId, EstimatedTimeViewModel model);
    Task<OperationResult<bool>> SpendTime(Guid userId, Guid taskId, DurationViewModel model);
    Task<OperationResult<bool>> Vote(Guid userId, Guid taskId, VoteViewModel model);
    Task<OperationResult<bool>> VoteSetting(Guid userId, Guid taskId, VoteSettingViewModel model);
    Task<OperationResult<bool>> VoteClear(Guid userId, Guid taskId);
    Task<OperationResult<bool>> SetDate(Guid userId, Guid taskId, SetDateViewModel model);
    Task<OperationResult<WorkPackageTaskViewModel[]>> Calendar(Guid userId, DurationViewModel model);
    Task<OperationResult<KartablViewModel>> Kartabl(Guid userId, DurationViewModel model);

    Task<OperationResult<AdvancedPlayerViewModel>> FetchAdvanced(Guid userId, Guid attachmentId);

    Task<OperationResult<AdvancedPlayerCommentViewModel>> CommentAdvanced(Guid userId, Guid attachmentId,
        EditAdvancedCommentViewModel model);

    Task<OperationResult<bool>> EditAdvancedComment(Guid userId, Guid commentId, TitleViewModel model);
    Task<OperationResult<bool>> RemoveAdvancedComment(Guid userId, Guid commentId);
    Task<OperationResult<PdfAdvancedCommentViewModel>> PdfAdvanced(Guid userId, Guid attachmentId);
    Task<OperationResult<UploadResultViewModel>> BulkAttachment(Guid userId, Guid id, UploadedFileViewModel file);
    Task<OperationResult<BulkDownloadResultViewModel>> BulkDownload(Guid userId, Guid id, Guid[] picked);
}