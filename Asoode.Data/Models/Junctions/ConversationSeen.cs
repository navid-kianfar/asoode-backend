using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class ConversationSeen : BaseEntity
{
    [Required] public Guid ConversationId { get; set; }
    [Required] public Guid UserId { get; set; }
}