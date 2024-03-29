using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.Admin;

public class DiscountEditableViewModel
{
    [MaxLength(500)] public string Title { get; set; }
    [MaxLength(100)] public string Code { get; set; }
    [MaxLength(1500)] public string Description { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public int MaxUsage { get; set; }
    public int Percent { get; set; }
    public double MaxUnit { get; set; }
    public Guid? ForUser { get; set; }
    public Guid? PlanId { get; set; }
    public CostUnit Unit { get; set; }
}