using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Order;

public record CheckDiscountDto
{
    [Required] [MaxLength(100)] public string Code { get; set; }
    [Required] public double Amount { get; set; }
    [Required] public Guid PlanId { get; set; }
}