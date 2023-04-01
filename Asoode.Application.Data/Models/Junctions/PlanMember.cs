using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class PlanMember : BaseEntity
    {
        public Guid PlanId { get; set; }
        [MaxLength(200)] public string Identifier { get; set; }
    }
}