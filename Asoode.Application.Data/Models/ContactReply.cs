using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class ContactReply : BaseEntity
    {
        public Guid ContactId { get; set; }
        public Guid UserId { get; set; }
        [Required][MaxLength(2000)]public string Message { get; set; }
    }
}