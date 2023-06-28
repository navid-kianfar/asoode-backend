using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Wallet : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? OrderId { get; set; }
    public double Amount { get; set; }
    public WalletType Type { get; set; }
    public CostUnit Unit { get; set; }
}