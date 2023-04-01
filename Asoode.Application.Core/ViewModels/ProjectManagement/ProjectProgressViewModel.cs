namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class ProjectProgressViewModel
{
    public DateTime Date { get; set; }
    public int Created { get; set; }
    public int Done { get; set; }
    public int Blocked { get; set; }
}