using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Core.ViewModels.General.Search;

public class SearchStorageViewModel
{
    public SearchStorageViewModel()
    {
        Files = Array.Empty<ExplorerFileViewModel>();
        Folders = Array.Empty<ExplorerFolderViewModel>();
    }

    public ExplorerFileViewModel[] Files { get; set; }
    public ExplorerFolderViewModel[] Folders { get; set; }
}