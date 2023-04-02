using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class Project : BaseEntity
    {
        [Required] public Guid UserId { get; set; }
        [Required] [MaxLength(2000)] public string Title { get; set; }
        [MaxLength(2000)] public string Description { get; set; }
        public bool Complex { get; set; }
        public bool Premium { get; set; }
        public ProjectTemplate Template { get; set; }
        public Guid PlanInfoId { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }
}