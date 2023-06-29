namespace Asoode.Shared.Abstraction.Dtos.Marketer;

public record MarketerDto : BaseDto
{
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public int? Fixed { get; set; }
    public int? Percent { get; set; }
    public string Title { get; set; }
    public int Index { get; set; }
}