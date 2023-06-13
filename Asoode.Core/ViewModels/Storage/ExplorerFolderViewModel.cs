using System;

namespace Asoode.Core.ViewModels.Storage;

public class ExplorerFolderViewModel
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Path { get; set; }
    public string Parent { get; set; }
}