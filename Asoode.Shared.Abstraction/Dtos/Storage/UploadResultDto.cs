namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record UploadResultDto
{
    public bool AttachmentSize { get; set; }
    public bool StorageSize { get; set; }
    public bool Success { get; set; }
}