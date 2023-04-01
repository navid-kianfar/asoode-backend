using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.Membership.Order;

public class RequestOrderViewModel
{
    [Required] public Guid PlanId { get; set; }
    [Required] public bool PayNow { get; set; }
    [Required] public OrderType Type { get; set; }
    [Required] public OrderDuration Duration { get; set; }
    [Required] public bool UseWallet { get; set; }
    [MaxLength(100)] public string DiscountCode { get; set; }
    public int Users { get; set; }
    public int WorkPackage { get; set; }
    public int Project { get; set; }
    public int ComplexGroup { get; set; }
    public int SimpleGroup { get; set; }
    public double DiskSpace { get; set; }
}