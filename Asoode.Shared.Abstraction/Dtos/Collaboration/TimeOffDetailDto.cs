using Asoode.Shared.Abstraction.Dtos.ProjectManagement;

namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record TimeOffDetailDto
{
    public WorkPackageTaskDto[] Tasks { get; set; }
}