using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record ProjectCreateDto : CreateWorkPackageDto
{
    public ProjectTemplate Template { get; set; }
    public bool Complex { get; set; }
    public bool Import { get; set; }
}