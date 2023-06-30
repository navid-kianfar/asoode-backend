using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record ProjectPrepareDto
{
    public ProjectDto Dto { get; set; }
    public PendingInvitationDto[] PendingInvitations { get; set; }
    public MemberInfoDto[] AllInvited { get; set; }
    public List<InviteDto> InviteById { get; set; }
    public string[] EmailIdentities { get; set; }
    public MemberInfoDto User { get; set; }
    public dynamic Plan { get; set; }
}