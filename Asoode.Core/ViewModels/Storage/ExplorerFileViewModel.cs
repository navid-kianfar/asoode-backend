using System;
using System.IO;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.Storage;

public record UploadedStorageItemDto : StorageItemDto
{
    public Guid Id { get; set; } = IncrementalGuid.NewId();
    public string Directory { get; set; }
    public string ThumbnailUrl { get; set; }
    public FileType Type { get; set; }

    public UploadViewModel ToViewModel()
    {
        return new UploadViewModel
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

public record StorageItemDto
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