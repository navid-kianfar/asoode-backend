using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class WorkPackageMember : BaseEntity
{
    public bool IsGroup { get; set; }
    public Guid RecordId { get; set; }
    public Guid PackageId { get; set; }
    public AccessType Access { get; set; }
    public Guid ProjectId { get; set; }

    public WorkPackageMemberDto ToDto()
    {
        return new WorkPackageMemberDto
        {
            Access = Access,
            PackageId = PackageId,
            RecordId = RecordId,
            IsGroup = IsGroup,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            // BlockNotification = BlockNotification,
            // ShowStats = ShowStats,
        };
    }
}