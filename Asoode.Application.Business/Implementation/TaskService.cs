using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Reports;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class TaskService : ITaskService
{
    public Task<OperationResult<bool>> Create(Guid userId, Guid packageId, CreateTaskDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Reposition(Guid userId, Guid taskId, RepositionDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Move(Guid userId, Guid taskId, MoveTaskDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<WorkPackageTaskDto>> Detail(Guid userId, Guid taskId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Comment(Guid userId, Guid taskId, PostTaskCommentDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangeTitle(Guid userId, Guid id, TitleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangeDescription(Guid userId, Guid id, TitleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangeState(Guid userId, Guid id, StateDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> AddMember(Guid userId, Guid id, TaskMemberDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveMember(Guid userId, Guid taskId, Guid recordId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> AddLabel(Guid userId, Guid taskId, Guid labelId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveLabel(Guid userId, Guid taskId, Guid labelId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UploadResultDto>> AddAttachment(Guid userId, Guid taskId, StorageItemDto file)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveAttachment(Guid userId, Guid attachmentId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RenameAttachment(Guid userId, Guid attachmentId, TitleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Archive(Guid userId, Guid taskId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Watch(Guid userId, Guid taskId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> CoverAttachment(Guid userId, Guid attachmentId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Location(Guid userId, Guid taskId, LocationDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<TaskLogDto[]>> Logs(Guid userId, Guid taskId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Estimated(Guid userId, Guid taskId, EstimatedTimeDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> SpendTime(Guid userId, Guid taskId, DurationDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Vote(Guid userId, Guid taskId, VoteDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> VoteSetting(Guid userId, Guid taskId, VoteSettingDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> VoteClear(Guid userId, Guid taskId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> SetDate(Guid userId, Guid taskId, SetDateDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<WorkPackageTaskDto[]>> Calendar(Guid userId, DurationDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<KartablDto>> Kartabl(Guid userId, DurationDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<AdvancedPlayerDto>> FetchAdvanced(Guid userId, Guid attachmentId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<AdvancedPlayerCommentDto>> CommentAdvanced(Guid userId, Guid attachmentId, EditAdvancedCommentDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditAdvancedComment(Guid userId, Guid commentId, TitleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveAdvancedComment(Guid userId, Guid commentId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<PdfAdvancedCommentDto>> PdfAdvanced(Guid userId, Guid attachmentId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UploadResultDto>> BulkAttachment(Guid userId, Guid id, StorageItemDto file)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<BulkDownloadResultDto>> BulkDownload(Guid userId, Guid id, Guid[] picked)
    {
        throw new NotImplementedException();
    }
}