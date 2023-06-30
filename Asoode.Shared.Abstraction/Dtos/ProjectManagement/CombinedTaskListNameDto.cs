namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CombinedTaskListNameDto
{
    public WorkPackageTaskDto Task { get; set; }
    public string List { get; set; }
}