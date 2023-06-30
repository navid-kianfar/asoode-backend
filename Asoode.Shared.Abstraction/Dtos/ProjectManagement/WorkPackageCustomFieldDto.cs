using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageCustomFieldDto
{
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public bool Show { get; set; }
    public string Title { get; set; }
    public WorkPackageCustomFieldType Type { get; set; }
}