using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CombinedTaskMemberUserDto
{
    public WorkPackageTaskMemberDto Member { get; set; }
    public MemberInfoDto User { get; set; }
}