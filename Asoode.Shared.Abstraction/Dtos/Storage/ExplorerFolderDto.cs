namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record ExplorerFolderDto
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Path { get; set; }
    public string Parent { get; set; }
}