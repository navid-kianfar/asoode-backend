using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Helpers;

namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record UploadDto
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


    public ExplorerFileDto ToExplorerDto()
    {
        var ext = System.IO.Path.GetExtension(Path);
        return new ExplorerFileDto
        {
            Name = Name,
            ExtensionLessName = Name,
            CreatedAt = CreatedAt,
            Extension = ext,
            Size = 0,
            Url = Path,
            IsDocument = IOHelper.IsDocument(ext),
            IsImage = IOHelper.IsImage(ext),
            IsPdf = IOHelper.IsPdf(ext),
            IsPresentation = IOHelper.IsPresentation(ext),
            IsSpreadsheet = IOHelper.IsSpreadsheet(ext),
            IsArchive = IOHelper.IsArchive(ext),
            IsExecutable = IOHelper.IsExecutable(ext),
            IsCode = IOHelper.IsCode(ext),
            IsOther = IOHelper.IsOther(ext),
        };
    }
}