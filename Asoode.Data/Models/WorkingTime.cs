using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class WorkingTime : BaseEntity
{
    [Required] public DateTime BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
    [Required] public Guid GroupId { get; set; }
    [Required] public Guid UserId { get; set; }
}