using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class ConversationSeen : BaseEntity
{
    [Required] public Guid ConversationId { get; set; }
    [Required] public Guid UserId { get; set; }
}