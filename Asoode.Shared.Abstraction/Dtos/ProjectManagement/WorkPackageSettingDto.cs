using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageSettingDto
{
    public WorkPackageTaskVisibility? Visibility { get; set; }
}