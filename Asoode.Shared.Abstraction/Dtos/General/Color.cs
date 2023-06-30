namespace Asoode.Shared.Abstraction.Dtos.General;

public record Color
{
    public bool Dark { get; set; }
    public bool Enabled { get; set; }
    public Guid Id { get; set; }
    public string LatinTitle { get; set; }
    public string Title { get; set; }
    public string Value { get; set; }
}