namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record KartablDto
{
    public WorkPackageTaskDto[] Tasks { get; set; }
}