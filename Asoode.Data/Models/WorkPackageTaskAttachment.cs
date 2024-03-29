using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class WorkPackageTaskAttachment : BaseEntity
{
    [Required] public Guid PackageId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    [Required] public Guid TaskId { get; set; }

    public bool IsCover { get; set; }
    public Guid? UploadId { get; set; }
    [MaxLength(500)] [Required] public string Path { get; set; }
    [MaxLength(500)] public string ThumbnailPath { get; set; }
    [MaxLength(150)] [Required] public string Title { get; set; }
    [MaxLength(500)] public string Description { get; set; }
    [Required] public Guid UserId { get; set; }
    [Required] public WorkPackageTaskAttachmentType Type { get; set; }
}