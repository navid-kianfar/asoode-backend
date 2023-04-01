using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class ProjectMember : BaseEntity
    {
        public bool IsGroup { get; set; }
        public Guid RecordId { get; set; }
        public Guid ProjectId { get; set; }
        public AccessType Access { get; set; }
        public bool BlockNotification { get; set; }
    }
}