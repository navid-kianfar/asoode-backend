namespace Asoode.Application.Core.ViewModels.Storage;

public class UploadedFileViewModel
{
    public Stream FileStream { get; set; }
    public string FileName { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; }
}