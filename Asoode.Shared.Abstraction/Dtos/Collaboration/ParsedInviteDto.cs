using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record ParsedInviteDto
{
    public bool OverCapacity { get; set; }
    public string[] EmailIdentities { get; set; }
    public List<InviteDto> InviteById { get; set; }
    public List<InviteDto> InviteByEmail { get; set; }
    public Guid[] IdIdentities { get; set; }
    public MemberInfoDto[] AllInvited { get; set; }
    public string[] NewMembers { get; set; }
}