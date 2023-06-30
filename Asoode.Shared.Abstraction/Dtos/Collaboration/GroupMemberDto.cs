using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record GroupMemberDto : BaseDto
{
    public MemberInfoDto Member { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public AccessType Access { get; set; }
}