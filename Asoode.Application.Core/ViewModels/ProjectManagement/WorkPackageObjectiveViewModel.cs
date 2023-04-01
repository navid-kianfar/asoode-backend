using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageObjectiveViewModel : BaseViewModel
{
    public Guid PackageId { get; set; }
    public Guid? ParentId { get; set; }
    public int? Order { get; set; }
    public int? Level { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public WorkPackageObjectiveType Type { get; set; }
}