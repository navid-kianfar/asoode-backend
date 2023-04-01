namespace Asoode.Application.Core.ViewModels.Storage;

public class UploadedFileViewModel
{
    public Stream FileStream { get; set; }
    public string FileName { get; set; }
    public int Size { get; set; }
}