namespace Asoode.Website.Abstraction.Dtos.General;

public record SiteMapDto
{
    public string Location { get; set; }
    public string Title { get; set; }
    public DateTime LastModified { get; set; }
    public string Priority { get; set; }
}