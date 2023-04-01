using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageListViewModel : BaseViewModel
{
    public Guid PackageId { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public bool DarkColor { get; set; }
    public int Order { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public bool Restricted { get; set; }
    public WorkPackageTaskState? Kanban { get; set; }
}