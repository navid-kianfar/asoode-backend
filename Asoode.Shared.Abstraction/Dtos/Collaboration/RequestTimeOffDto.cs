namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record RequestTimeOffDto
{
    public DateTime BeginAt { get; set; }
    public string Description { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsHourly { get; set; }
}