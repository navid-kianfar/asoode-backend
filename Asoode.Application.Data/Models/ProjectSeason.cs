using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class ProjectSeason : BaseEntity
    {
        [Required] public Guid UserId { get; set; }
        [Required] public Guid ProjectId { get; set; }
        [Required] [MaxLength(2000)] public string Title { get; set; }
        [MaxLength(2000)] public string Description { get; set; }
        public int Order { get; set; }
    }
}