using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class ProjectCreateViewModel : CreateWorkPackageViewModel
{
    public ProjectTemplate Template { get; set; }
    public bool Complex { get; set; }
    public bool Import { get; set; }
}