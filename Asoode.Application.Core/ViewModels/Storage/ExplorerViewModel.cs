namespace Asoode.Application.Core.ViewModels.Storage;

public class ExplorerViewModel
{
    public ExplorerViewModel()
    {
        Files = Array.Empty<ExplorerFileViewModel>();
        Folders = Array.Empty<ExplorerFolderViewModel>();
    }

    public ExplorerFileViewModel[] Files { get; set; }
    public ExplorerFolderViewModel[] Folders { get; set; }
}