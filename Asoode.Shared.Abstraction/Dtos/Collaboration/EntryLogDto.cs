namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record EntryLogDto : BaseDto
{
    public DateTime BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string Duration { get; set; }
    public int Index { get; set; }

    public string FullName { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
}