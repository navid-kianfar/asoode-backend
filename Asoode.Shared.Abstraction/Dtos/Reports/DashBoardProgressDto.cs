namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record DashBoardProgressDto : DashBoardOverallDto
{
    public DateTime Date { get; set; }
}