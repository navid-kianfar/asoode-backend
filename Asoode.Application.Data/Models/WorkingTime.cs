using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class WorkingTime : BaseEntity
    {
        [Required] public DateTime BeginAt { get; set; }
        public DateTime? EndAt { get; set; }
        [Required] public Guid GroupId { get; set; }
        [Required] public Guid UserId { get; set; }
    }
}