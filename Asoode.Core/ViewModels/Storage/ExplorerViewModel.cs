namespace Asoode.Core.ViewModels.Storage;

public class ExplorerViewModel
{
    public ExplorerViewModel()
    {
        Files = new ExplorerFileViewModel[0];
        Folders = new ExplorerFolderViewModel[0];
    }

    public ExplorerFileViewModel[] Files { get; set; }
    public ExplorerFolderViewModel[] Folders { get; set; }
}