namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record SetDateDto
{
    public DateTime? DueAt { get; set; }
    public DateTime? BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
}