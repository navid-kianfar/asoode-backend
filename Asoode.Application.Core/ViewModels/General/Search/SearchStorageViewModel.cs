using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Core.ViewModels.General.Search;

public class SearchStorageViewModel
{
    public SearchStorageViewModel()
    {
        Files = new ExplorerFileViewModel[0];
        Folders = new ExplorerFolderViewModel[0];
    }

    public ExplorerFileViewModel[] Files { get; set; }
    public ExplorerFolderViewModel[] Folders { get; set; }
}