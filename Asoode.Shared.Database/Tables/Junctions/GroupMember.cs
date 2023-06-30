﻿using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class GroupMember : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public AccessType Access { get; set; }
    public Guid RootId { get; set; }
    public int Level { get; set; }

    public GroupMemberDto ToDto()
    {
        return new GroupMemberDto
        {
            Access = Access,
            GroupId = GroupId,
            UserId = UserId,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }
}