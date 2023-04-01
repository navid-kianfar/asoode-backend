using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class WorkPackageMember : BaseEntity
    {
        public bool IsGroup { get; set; }
        public Guid RecordId { get; set; }
        public Guid PackageId { get; set; }
        public AccessType Access { get; set; }
        public Guid ProjectId { get; set; }
    }
}