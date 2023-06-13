using System;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class RenameAttachmentViewModel
{
    public string Path { get; set; }
    public string Name { get; set; }
    public UploadSection Section { get; set; }
    public Guid? UserId { get; set; }
}