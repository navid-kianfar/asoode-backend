namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageCustomFieldItemDto
{
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public Guid CustomFieldId { get; set; }
    public string Color { get; set; }
    public int Order { get; set; }
    public string Title { get; set; }
}