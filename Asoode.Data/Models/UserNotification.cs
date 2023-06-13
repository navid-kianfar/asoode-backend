using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class UserNotification : BaseEntity
{
    [Required] [MaxLength(2000)] public string Title { get; set; }
    [Required] [MaxLength(2000)] public string Description { get; set; }
    public bool Seen { get; set; }
    public Guid UserId { get; set; }
    public Guid ActivityUserId { get; set; }
}