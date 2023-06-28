using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class MarketerIncome : BaseEntity
{
    [Required] public double BillAmount { get; set; }
    [Required] public DateTime? ClearedAt { get; set; }
    [Required] public double EarnAmount { get; set; }
    [Required] public int Fixed { get; set; }
    [Required] public Guid MarketerId { get; set; }
    [Required] public Guid PaymentId { get; set; }
    [Required] public int Percent { get; set; }
}