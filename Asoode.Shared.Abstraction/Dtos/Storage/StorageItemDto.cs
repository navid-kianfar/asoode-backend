namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record StorageItemDto
{
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public MemoryStream? Stream { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string FileField { get; set; } = string.Empty;
}