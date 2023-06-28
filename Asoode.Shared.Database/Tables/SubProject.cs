﻿using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class SubProject : BaseEntity
{
    [Required] public Guid UserId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? ParentId { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    public int Level { get; set; }
    public int Order { get; set; }
}