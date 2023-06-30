using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class ProjectMember : BaseEntity
{
    public bool IsGroup { get; set; }
    public Guid RecordId { get; set; }
    public Guid ProjectId { get; set; }
    public AccessType Access { get; set; }
    public bool BlockNotification { get; set; }

    public ProjectMemberDto ToDto()
    {
        return new ProjectMemberDto
        {
            Access = Access,
            ProjectId = ProjectId,
            RecordId = RecordId,
            IsGroup = IsGroup,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            // Member = Member,
        };
    }
}