namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageTaskLabelDto : BaseDto
{
    public Guid TaskId { get; set; }
    public Guid LabelId { get; set; }
    public Guid PackageId { get; set; }
}