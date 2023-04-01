using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class WorkPackageTaskAttachmentViewModel : BaseViewModel
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
}