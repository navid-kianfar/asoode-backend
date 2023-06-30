using Asoode.Shared.Abstraction.Dtos.Storage;

namespace Asoode.Shared.Abstraction.Dtos.General.Search;

public record SearchStorageDto
{
    public ExplorerFileDto[] Files { get; set; }
    public ExplorerFolderDto[] Folders { get; set; }
}