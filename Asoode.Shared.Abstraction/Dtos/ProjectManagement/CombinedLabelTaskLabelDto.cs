using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CombinedLabelTaskLabelDto
{
    public WorkPackageTaskLabelDto TaskLabel { get; set; }
    public LabelDto Label { get; set; }
}