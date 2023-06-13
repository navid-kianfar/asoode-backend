using System;
using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Membership.Order;

public class CheckDiscountViewModel
{
    [Required] [MaxLength(100)] public string Code { get; set; }
    [Required] public double Amount { get; set; }
    [Required] public Guid PlanId { get; set; }
}