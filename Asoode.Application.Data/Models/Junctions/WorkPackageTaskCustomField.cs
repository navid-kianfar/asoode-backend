using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class WorkPackageTaskCustomField : BaseEntity
    {
        [Required] public Guid CustomFieldId { get; set; }
        [Required] public Guid TaskId { get; set; }
        [Required] public Guid PackageId { get; set; }
        [Required] public Guid ProjectId { get; set; }
        public Guid? SubProjectId { get; set; }

        public bool? BoolValue { get; set; }
        public DateTime? DateValue { get; set; }
        public Guid? ItemValue { get; set; }
        public int? NumberValue { get; set; }
        [MaxLength(2000)] public string StringValue { get; set; }
        public WorkPackageCustomFieldType Type { get; set; }
    }
}