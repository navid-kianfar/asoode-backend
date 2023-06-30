namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record TimeSpentDto
{
    public WorkPackageTaskDto Task { get; set; }
    public WorkPackageTaskTimeDto Time { get; set; }
}