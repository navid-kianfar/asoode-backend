using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class WorkPackageLabel : BaseEntity
    {
        [Required] public Guid PackageId { get; set; }
        [MaxLength(250)] public string Title { get; set; }

        [MaxLength(100)] public string Color { get; set; }
        public bool DarkColor { get; set; }
    }
}