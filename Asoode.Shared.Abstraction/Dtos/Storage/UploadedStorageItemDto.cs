using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record UploadedStorageItemDto : StorageItemDto
{
    public Guid Id { get; set; } = IncrementalGuid.NewId();
    public string Directory { get; set; }
    public string ThumbnailUrl { get; set; }
    public FileType Type { get; set; }

    public UploadDto ToDto()
    {
        return new UploadDto
        {
            RecordId = RecordId,
            UserId = UserId,
            Directory = Directory,
            Extension = Extension,
            Name = FileName,
            Path = Url,
            CreatedAt = CreatedAt,
            Section = Section,
            Size = FileSize,
            Type = Type,
            Id = Id,
            Public = false,
            ThumbnailPath = ThumbnailUrl
        };
    }
}