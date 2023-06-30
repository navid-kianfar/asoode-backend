using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageObjectiveDto : BaseDto
{
    public Guid PackageId { get; set; }
    public Guid? ParentId { get; set; }
    public int? Order { get; set; }
    public int? Level { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public WorkPackageObjectiveType Type { get; set; }
}