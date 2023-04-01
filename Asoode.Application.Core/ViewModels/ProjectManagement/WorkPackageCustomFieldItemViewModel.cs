namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageCustomFieldItemViewModel
{
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public Guid CustomFieldId { get; set; }
    public string Color { get; set; }
    public int Order { get; set; }
    public string Title { get; set; }
}