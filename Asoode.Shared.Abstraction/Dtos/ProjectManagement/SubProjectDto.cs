using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record SubProjectDto : BaseDto
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Level { get; set; }
    public int Order { get; set; }
}