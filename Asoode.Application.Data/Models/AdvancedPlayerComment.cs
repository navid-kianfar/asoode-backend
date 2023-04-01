using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class AdvancedPlayerComment : BaseEntity
    {
        public Guid AttachmentId { get; set; }
        public Guid UserId { get; set; }
        public float StartFrame { get; set; }
        public float? EndFrame { get; set; }
        [MaxLength(1000)]public string Message { get; set; }
        [MaxLength(1000)]public string Payload { get; set; }
    }
}