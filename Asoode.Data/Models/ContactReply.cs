using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class ContactReply : BaseEntity
{
    public Guid ContactId { get; set; }
    public Guid UserId { get; set; }
    [Required] [MaxLength(2000)] public string Message { get; set; }
}