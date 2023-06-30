using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record ExtendedStorageItemDto
{
    public MemoryStream Stream { get; set; }
    public string FileField { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public UploadSection Section { get; set; }
    public Guid PlanId { get; set; }
    public Guid RecordId { get; set; }
    public Guid UserId { get; set; }
    public string LocalFile { get; set; } = string.Empty;

    public string GetPath()
    {
        return $"{UserId}/{PlanId}/{DateTime.UtcNow:yyyy-MM-dd}/{RecordId}/{IncrementalGuid.NewId()}";
    }
}