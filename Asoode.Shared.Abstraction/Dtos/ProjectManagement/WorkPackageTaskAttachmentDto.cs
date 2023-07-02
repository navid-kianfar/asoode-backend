using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Helpers;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageTaskAttachmentDto : BaseDto
{
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public Guid TaskId { get; set; }

    public bool IsCover { get; set; }
    public Guid? UploadId { get; set; }
    public string Path { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
    public WorkPackageTaskAttachmentType Type { get; set; }
    public string ThumbnailPath { get; set; }

    public ExplorerFileDto ToExplorerDto()
    {
        var ext = System.IO.Path.GetExtension(Path);
        return new ExplorerFileDto
        {
            Name = Title,
            ExtensionLessName = Title,
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