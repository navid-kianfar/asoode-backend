namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record DashBoardOverallDto
{
    public int Total { get; set; }
    public int Done { get; set; }
    public int Blocked { get; set; }
    public int InProgress { get; set; }
}