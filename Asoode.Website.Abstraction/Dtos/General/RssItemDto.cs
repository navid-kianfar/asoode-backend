namespace Asoode.Website.Abstraction.Dtos.General;

public record RssItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
    public DateTime CreatedAt { get; set; }
}