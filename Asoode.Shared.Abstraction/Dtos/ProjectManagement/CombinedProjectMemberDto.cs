namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CombinedProjectMemberDto
{
    public ProjectDto Project { get; set; }
    public ProjectMemberDto Member { get; set; }
}