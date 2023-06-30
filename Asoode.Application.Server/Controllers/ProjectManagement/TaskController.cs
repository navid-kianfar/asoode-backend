using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.ProjectManagement;


[Route("v2/tasks")]
[ApiExplorerSettings(GroupName = "Tasks")]
public class TaskController : BaseController
{
    private readonly ITaskBiz _taskBiz;

    public TaskController(ITaskBiz taskBiz)
    {
        _taskBiz = taskBiz;
    }

    [HttpPost("{id:guid}/{userId:guid}/bulk-download")]
    [JwtAuthorize(UserType.Anonymous)]
    public async Task<IActionResult> BulkDownload(Guid id, Guid userId)
    {
        var picked = Request.Form
            .Select(i => Guid.Parse(i.Value))
            .ToArray();
        if (!picked.Any()) return NotFound();
        var op = await _taskBiz.BulkDownload(userId, id, picked);
        if (op.Status != OperationResultStatus.Success || op.Data.Zip == null) return NotFound();
        return File(op.Data.Zip, "application/zip", $"{op.Data.Title}_{DateTime.UtcNow.GetTime()}.zip");
    }

    [HttpPost("{id:guid}/create")]
    
    public async Task<IActionResult> Creat(Guid id, [FromBody] CreateTaskDto model)
    {
        var op = await _taskBiz.Create(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/attach")]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> AddAttachment(Guid id)
    {
        var file = await Request.Form.Files.First().ToStorageItem();
        var op = await _taskBiz.AddAttachment(_identity.User!.UserId, id, file);
        return Json(op);
    }

    [HttpPost("{id:guid}/bulk-attach")]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> BulkAttachment(Guid id)
    {
        var file = await Request.Form.Files.First().ToStorageItem();
        var op = await _taskBiz.BulkAttachment(_identity.User!.UserId, id, file);
        return Json(op);
    }

    [HttpPost("{id:guid}/watch")]
    
    public async Task<IActionResult> Watch(Guid id)
    {
        var op = await _taskBiz.Watch(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/archive")]
    
    public async Task<IActionResult> Archive(Guid id)
    {
        var op = await _taskBiz.Archive(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("attachment/{id:guid}/rename")]
    
    public async Task<IActionResult> RenameAttachment(Guid id, [FromBody] TitleDto model)
    {
        var op = await _taskBiz.RenameAttachment(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("attachment/{id:guid}/advanced")]
    public async Task<IActionResult> FetchAdvanced(Guid id)
    {
        var op = await _taskBiz.FetchAdvanced(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("attachment/{id:guid}/advanced/comment")]
    
    public async Task<IActionResult> CommentAdvanced(Guid id, [FromBody] EditAdvancedCommentDto model)
    {
        var op = await _taskBiz.CommentAdvanced(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("attachment/advanced/{id:guid}/edit-comment")]
    
    public async Task<IActionResult> EditAdvancedComment(Guid id, [FromBody] TitleDto model)
    {
        var op = await _taskBiz.EditAdvancedComment(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("attachment/advanced/{id:guid}/remove-comment")]
    public async Task<IActionResult> RemoveAdvancedComment(Guid id)
    {
        var op = await _taskBiz.RemoveAdvancedComment(_identity.User!.UserId, id);
        return Json(op);
    }

    [JwtAuthorize(UserType.Anonymous)]
    [HttpGet("attachment/advanced/{id:guid}/pdf")]
    public async Task<IActionResult> PdfAdvanced(Guid id)
    {
        // var op = await _taskBiz.PdfAdvancedComment(_identity.User!.UserId, id);
        var op = await _taskBiz.PdfAdvanced(Guid.Empty, id);
        if (op.Status != OperationResultStatus.Success) return NotFound();
        return File(op.Data.Stream, "application/pdf", op.Data.FileName);
    }

    [HttpPost("attachment/{id:guid}/remove")]
    
    public async Task<IActionResult> RemoveAttachment(Guid id)
    {
        var op = await _taskBiz.RemoveAttachment(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("attachment/{id:guid}/cover")]
    
    public async Task<IActionResult> CoverAttachment(Guid id)
    {
        var op = await _taskBiz.CoverAttachment(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/reposition")]
    
    public async Task<IActionResult> Reposition(Guid id, [FromBody] RepositionDto model)
    {
        var op = await _taskBiz.Reposition(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/move")]
    
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveTaskDto model)
    {
        var op = await _taskBiz.Move(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/location")]
    
    public async Task<IActionResult> Location(Guid id, [FromBody] LocationDto model)
    {
        var op = await _taskBiz.Location(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/detail")]
    
    public async Task<IActionResult> Detail(Guid id)
    {
        var op = await _taskBiz.Detail(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/comment")]
    
    public async Task<IActionResult> Comment(Guid id, [FromBody] PostTaskCommentDto model)
    {
        var op = await _taskBiz.Comment(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/estimated")]
    
    public async Task<IActionResult> Estimated(Guid id, [FromBody] EstimatedTimeDto model)
    {
        var op = await _taskBiz.Estimated(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/spend-time")]
    
    public async Task<IActionResult> SpendTime(Guid id, [FromBody] DurationDto model)
    {
        var op = await _taskBiz.SpendTime(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/change-title")]
    
    public async Task<IActionResult> ChangeTitle(Guid id, [FromBody] TitleDto model)
    {
        var op = await _taskBiz.ChangeTitle(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/vote")]
    
    public async Task<IActionResult> Vote(Guid id, [FromBody] VoteDto model)
    {
        var op = await _taskBiz.Vote(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/vote/setting")]
    
    public async Task<IActionResult> VoteSetting(Guid id, [FromBody] VoteSettingDto model)
    {
        var op = await _taskBiz.VoteSetting(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/vote/clear")]
    
    public async Task<IActionResult> VoteClear(Guid id)
    {
        var op = await _taskBiz.VoteClear(_identity.User!.UserId, id);
        return Json(op);
    }

    [HttpPost("{id:guid}/set-date")]
    
    public async Task<IActionResult> SetDate(Guid id, [FromBody] SetDateDto model)
    {
        var op = await _taskBiz.SetDate(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/change-description")]
    
    public async Task<IActionResult> ChangeDescription(Guid id, [FromBody] TitleDto model)
    {
        var op = await _taskBiz.ChangeDescription(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/change-state")]
    
    public async Task<IActionResult> ChangeState(Guid id, [FromBody] StateDto model)
    {
        var op = await _taskBiz.ChangeState(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{id:guid}/member/add")]
    
    public async Task<IActionResult> AddMember(Guid id, [FromBody] TaskMemberDto model)
    {
        var op = await _taskBiz.AddMember(_identity.User!.UserId, id, model);
        return Json(op);
    }

    [HttpPost("{taskId:guid}/member/{recordId:guid}/remove")]
    public async Task<IActionResult> RemoveMember(Guid taskId, Guid recordId)
    {
        var op = await _taskBiz.RemoveMember(_identity.User!.UserId, taskId, recordId);
        return Json(op);
    }

    [HttpPost("{taskId:guid}/label/add/{labelId:guid}")]
    public async Task<IActionResult> AddLabel(Guid taskId, Guid labelId)
    {
        var op = await _taskBiz.AddLabel(_identity.User!.UserId, taskId, labelId);
        return Json(op);
    }

    [HttpPost("{taskId:guid}/logs")]
    public async Task<IActionResult> Logs(Guid taskId)
    {
        var op = await _taskBiz.Logs(_identity.User!.UserId, taskId);
        return Json(op);
    }

    [HttpPost("{taskId:guid}/label/{labelId:guid}/remove")]
    public async Task<IActionResult> RemoveLabel(Guid taskId, Guid labelId)
    {
        var op = await _taskBiz.RemoveLabel(_identity.User!.UserId, taskId, labelId);
        return Json(op);
    }

    [HttpPost("calendar")]
    public async Task<IActionResult> Calendar([FromBody] DurationDto model)
    {
        var op = await _taskBiz.Calendar(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("kartabl")]
    public async Task<IActionResult> Kartabl([FromBody] DurationDto model)
    {
        var op = await _taskBiz.Kartabl(_identity.User!.UserId, model);
        return Json(op);
    }
}