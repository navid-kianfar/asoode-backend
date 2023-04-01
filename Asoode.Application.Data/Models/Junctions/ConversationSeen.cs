using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class ConversationSeen : BaseEntity
    {
        [Required] public Guid ConversationId { get; set; }
        [Required] public Guid UserId { get; set; }
    }
}