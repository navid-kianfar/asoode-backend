using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageLabelDto : BaseDto
{
    public Guid PackageId { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public bool DarkColor { get; set; }
}