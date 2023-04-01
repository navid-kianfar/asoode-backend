namespace Asoode.Application.Core.ViewModels.Storage;

public class ExplorerFileViewModel
{
    public string Name { get; set; }
    public string ExtensionLessName { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsImage { get; set; }
    public bool IsPdf { get; set; }
    public bool IsSpreadsheet { get; set; }
    public bool IsDocument { get; set; }
    public bool IsPresentation { get; set; }
    public bool IsArchive { get; set; }
    public bool IsExecutable { get; set; }
    public bool IsCode { get; set; }
    public bool IsOther { get; set; }
    public string Url { get; set; }
    public string Path { get; set; }
}