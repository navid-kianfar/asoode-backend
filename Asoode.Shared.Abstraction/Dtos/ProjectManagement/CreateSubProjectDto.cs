using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CreateSubProjectDto : SimpleDto
{
    public Guid? ParentId { get; set; }
}