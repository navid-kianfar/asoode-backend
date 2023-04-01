using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageLabelViewModel : BaseViewModel
{
    public Guid PackageId { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public bool DarkColor { get; set; }
}