namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record ExplorerDto
{
    public ExplorerDto()
    {
        Files = new ExplorerFileDto[0];
        Folders = new ExplorerFolderDto[0];
    }

    public ExplorerFileDto[] Files { get; set; }
    public ExplorerFolderDto[] Folders { get; set; }
}