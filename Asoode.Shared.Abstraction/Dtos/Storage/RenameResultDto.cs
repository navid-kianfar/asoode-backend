namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record RenameResultDto
{
    public string OldPath { get; set; }
    public string NewPath { get; set; }
    public bool Directory { get; set; }
}