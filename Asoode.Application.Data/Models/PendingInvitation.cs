using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class PendingInvitation : BaseEntity
    {
        public string Identifier { get; set; }
        public Guid RecordId { get; set; }
        public AccessType Access { get; set; }
        public PendingInvitationType Type { get; set; }
        public bool Canceled { get; set; }
    }
}