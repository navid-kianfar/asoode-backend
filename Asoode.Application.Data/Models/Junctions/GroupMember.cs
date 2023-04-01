using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class GroupMember : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public AccessType Access { get; set; }
        public Guid RootId { get; set; }
        public int Level { get; set; }
    }
}