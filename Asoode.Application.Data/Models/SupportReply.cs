using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class SupportReply : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid SupportId { get; set; }
        [Required][MaxLength(1000)]public string Message { get; set; }
        public bool Seen { get; set; }
    }
}