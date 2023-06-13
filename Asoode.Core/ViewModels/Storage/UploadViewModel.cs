using System;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.Storage;

public class UploadViewModel
{
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
    public string Directory { get; set; }
    public string Extension { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string ThumbnailPath { get; set; }
    public bool Public { get; set; }
    public Guid RecordId { get; set; }
    public UploadSection Section { get; set; }
    public long Size { get; set; }
    public FileType Type { get; set; }
    public Guid UserId { get; set; }
}