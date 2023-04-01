using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class SupportContact : BaseEntity
    {
        public Guid? UserId { get; set; }
        public SupportType Type { get; set; }
        [Required][MaxLength(2)]public string Culture { get; set; }
        [Required][MaxLength(100)]public string Email { get; set; }
        [Required][MaxLength(250)]public string FullName { get; set; }
        [Required][MaxLength(500)]public string Subject { get; set; }
        [Required][MaxLength(1000)]public string Message { get; set; }
        public bool Seen { get; set; }
    }
}