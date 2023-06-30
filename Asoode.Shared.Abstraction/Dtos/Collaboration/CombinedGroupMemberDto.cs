namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record CombinedGroupMemberDto
{
    public GroupDto Group { get; set; }
    public GroupMemberDto Member { get; set; }
}