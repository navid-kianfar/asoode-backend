using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Discount;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Discount : BaseEntity
{
    [MaxLength(500)] public string Title { get; set; }
    [MaxLength(100)] public string Code { get; set; }
    [MaxLength(1500)] public string Description { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public Guid? ForUser { get; set; }
    public int MaxUsage { get; set; }
    public int Percent { get; set; }
    public double MaxUnit { get; set; }
    public CostUnit Unit { get; set; }
    public Guid? PlanId { get; set; }

    public DiscountDto ToDto()
    {
        return new DiscountDto
        {
            Code = Code,
            Description = Description,
            Percent = Percent,
            EndAt = EndAt,
            MaxUsage = MaxUsage,
            ForUser = ForUser,
            MaxUnit = MaxUnit,
            PlanId = PlanId,
            StartAt = StartAt,
            Title = Title,
            Unit = Unit,
            Id = Id,
            // Index = Index,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }
}