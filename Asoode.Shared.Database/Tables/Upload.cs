using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Upload : BaseEntity
{
    [MaxLength(2000)] public string Directory { get; set; }
    [Required] [MaxLength(10)] public string Extension { get; set; }
    [Required] [MaxLength(200)] public string Name { get; set; }
    [Required] [MaxLength(500)] public string Path { get; set; }
    [MaxLength(500)] public string ThumbnailPath { get; set; }
    public bool Public { get; set; }
    public Guid RecordId { get; set; }
    public UploadSection Section { get; set; }
    public long Size { get; set; }
    public FileType Type { get; set; }
    public Guid UserId { get; set; }

    public UploadDto ToDto()
    {
        return new UploadDto
        {
            CreatedAt = CreatedAt,
            Directory = Directory,
            Extension = Extension,
            RecordId = RecordId,
            ThumbnailPath = ThumbnailPath,
            Name = Name,
            UserId = UserId,
            Type = Type,
            Path = Path,
            Public = Public,
            Section = Section,
            Size = Size,
            Id = Id,
        };
    }
}