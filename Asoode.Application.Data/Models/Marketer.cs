using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class Marketer : BaseEntity
    {
        [Required] [MaxLength(100)] public string Code { get; set; }
        [Required] [MaxLength(2000)] public string Description { get; set; }
        public bool Enabled { get; set; }
        public int? Fixed { get; set; }
        public int? Percent { get; set; }
        [Required] [MaxLength(2000)] public string Title { get; set; }
    }
}