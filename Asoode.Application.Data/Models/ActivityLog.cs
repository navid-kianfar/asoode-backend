using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class ActivityLog : BaseEntity
    {
        [Required] [MaxLength(2000)] public string Description { get; set; }
        public Guid RecordId { get; set; }
        public ActivityType Type { get; set; }
        public Guid UserId { get; set; }
    }
}