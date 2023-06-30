namespace Asoode.Shared.Abstraction.Dtos.General;

public record ChangeColorDto
{
    public string Color { get; set; }
    public bool DarkColor { get; set; }
}