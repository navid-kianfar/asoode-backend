using System.IO;

namespace Asoode.Core.ViewModels.Storage;

public class UploadResultViewModel
{
    public bool AttachmentSize { get; set; }
    public bool StorageSize { get; set; }
    public bool Success { get; set; }
}

public class BulkDownloadResultViewModel
{
    public Stream Zip { get; set; }
    public string Title { get; set; }
}