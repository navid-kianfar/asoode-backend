namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record WorkPackageFilterDto
{
    public bool Mine { get; set; }
    public bool Archived { get; set; }
    public bool Active { get; set; }

    public Dictionary<Guid, bool> Labels { get; set; }
}