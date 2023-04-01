using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class WorkPackageListMember : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ListId { get; set; }

        public bool Chart { get; set; }
    }
}