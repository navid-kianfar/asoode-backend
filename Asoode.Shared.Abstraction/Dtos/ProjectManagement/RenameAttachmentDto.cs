using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record RenameAttachmentDto
{
    public string Path { get; set; }
    public string Name { get; set; }
    public UploadSection Section { get; set; }
    public Guid? UserId { get; set; }
}