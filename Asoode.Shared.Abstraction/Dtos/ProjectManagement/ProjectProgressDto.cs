namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record ProjectProgressDto
{
    public DateTime Date { get; set; }
    public int Created { get; set; }
    public int Done { get; set; }
    public int Blocked { get; set; }
}