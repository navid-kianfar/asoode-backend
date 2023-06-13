using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class Project : BaseEntity
{
    [Required] public Guid UserId { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    public bool Complex { get; set; }
    public bool Premium { get; set; }
    public ProjectTemplate Template { get; set; }
    public Guid PlanInfoId { get; set; }
    public DateTime? ArchivedAt { get; set; }
}