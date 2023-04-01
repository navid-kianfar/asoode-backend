using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class WorkPackageView : BaseEntity
    {
        [Required] public Guid UserId { get; set; }
        public Guid? PackageId { get; set; }
        public Guid? FieldId { get; set; }
        public int Order { get; set; }
        public int Width { get; set; }
        public WorkPackageStaticFields Type { get; set; }
    }
}