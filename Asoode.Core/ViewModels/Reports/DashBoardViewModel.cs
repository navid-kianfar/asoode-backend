namespace Asoode.Core.ViewModels.Reports;

public class DashBoardViewModel
{
    public DashBoardEventViewModel[] Events { get; set; }
    public DashBoardOverallViewModel Overall { get; set; }
    public DashBoardProgressViewModel[] Progress { get; set; }
}