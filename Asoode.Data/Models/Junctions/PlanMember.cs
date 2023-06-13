using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class PlanMember : BaseEntity
{
    public Guid PlanId { get; set; }
    [MaxLength(200)] public string Identifier { get; set; }
}