namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CombinedWorkPackageMemberDto
{
    public WorkPackageDto Package { get; set; }
    public WorkPackageMemberDto Member { get; set; }
}