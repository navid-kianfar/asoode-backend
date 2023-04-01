namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageProgressViewModel
{
    public int Percent => Total == 0 ? 0 : (Done + CancelOrDuplicate) * 100 / Total;
    public int Total { get; set; }
    public int Done { get; set; }
    public int CancelOrDuplicate { get; set; }
}