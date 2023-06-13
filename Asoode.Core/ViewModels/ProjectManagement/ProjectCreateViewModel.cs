using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class ProjectCreateViewModel : CreateWorkPackageViewModel
{
    public ProjectTemplate Template { get; set; }
    public bool Complex { get; set; }
    public bool Import { get; set; }
}