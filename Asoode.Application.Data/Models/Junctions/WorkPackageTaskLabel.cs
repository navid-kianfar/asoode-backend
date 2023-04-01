using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class WorkPackageTaskLabel : BaseEntity
    {
        public Guid TaskId { get; set; }
        public Guid LabelId { get; set; }
        public Guid PackageId { get; set; }
    }
}