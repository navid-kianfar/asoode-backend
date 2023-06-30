namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record DayReportDto
{
    public DateTime Date { get; set; }
    public int Done { get; set; }
    public int Blocked { get; set; }
}