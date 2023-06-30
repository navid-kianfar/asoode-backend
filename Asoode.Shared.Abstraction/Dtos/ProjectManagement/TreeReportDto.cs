namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record TreeReportDto
{
    public double TimeSpent { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Total { get; set; }
    public int Done { get; set; }
}