using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class PlanMember : BaseEntity
{
    public Guid PlanId { get; set; }
    [MaxLength(200)] public string Identifier { get; set; }
}