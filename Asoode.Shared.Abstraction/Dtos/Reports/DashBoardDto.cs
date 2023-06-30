namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record DashBoardDto
{
    public DashBoardEventDto[] Events { get; set; }
    public DashBoardOverallDto Overall { get; set; }
    public DashBoardProgressDto[] Progress { get; set; }
}