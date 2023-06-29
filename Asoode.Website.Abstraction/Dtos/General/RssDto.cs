namespace Asoode.Website.Abstraction.Dtos.General;

public record RssDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
    public RssItemDto[] Items { get; set; }
    public string Location { get; set; }
}