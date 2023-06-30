namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageTaskCommentDto : BaseDto
{
    public string Message { get; set; }
    public bool Private { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ReplyId { get; set; }
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
}