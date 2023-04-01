using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class Discount : BaseEntity
    {
        [MaxLength(500)]public string Title { get; set; }
        [MaxLength(100)]public string Code { get; set; }
        [MaxLength(1500)]public string Description { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public Guid? ForUser { get; set; }
        public int MaxUsage { get; set; }
        public int Percent { get; set; }
        public double MaxUnit { get; set; }
        public CostUnit Unit { get; set; }
        public Guid? PlanId { get; set; }
    }
}