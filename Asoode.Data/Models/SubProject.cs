using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class SubProject : BaseEntity
{
    [Required] public Guid UserId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    public Guid? ParentId { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    public int Level { get; set; }
    public int Order { get; set; }
}