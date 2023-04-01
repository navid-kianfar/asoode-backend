namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageFilterViewModel
{
    public bool Mine { get; set; }
    public bool Archived { get; set; }
    public bool Active { get; set; }

    public Dictionary<Guid, bool> Labels { get; set; }
}