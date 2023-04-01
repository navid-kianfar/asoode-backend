using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class WebPush : BaseEntity
    {
        public bool Android { get; set; }
        [Required] [MaxLength(500)] public string Auth { get; set; }
        [MaxLength(50)] public string Browser { get; set; }
        public bool Desktop { get; set; }
        [MaxLength(50)] public string Device { get; set; }
        public bool Enabled { get; set; }
        [Required] [MaxLength(500)] public string Endpoint { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public bool Ios { get; set; }
        public bool Mobile { get; set; }
        [Required] [MaxLength(500)] public string P256dh { get; set; }
        [MaxLength(50)] public string Platform { get; set; }
        public bool Safari { get; set; }
        public bool Tablet { get; set; }
        [Required] public Guid UserId { get; set; }
        [MaxLength(200)]public string Title { get; set; }
    }
}