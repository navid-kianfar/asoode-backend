using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class Channel : BaseEntity
    {
        public DateTime? ArchivedAt { get; set; }
        [Required] [MaxLength(1000)] public string Title { get; set; }
        public ChannelType Type { get; set; }
        [Required] public Guid UserId { get; set; }
        public Guid RootId { get; set; }
    }
}