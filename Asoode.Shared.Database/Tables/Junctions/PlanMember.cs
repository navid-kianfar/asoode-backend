using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class PlanMember : BaseEntity
{
    public Guid PlanId { get; set; }
    [MaxLength(200)] public string Identifier { get; set; }
}