using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models;

public class Wallet : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? OrderId { get; set; }
    public double Amount { get; set; }
    public WalletType Type { get; set; }
    public CostUnit Unit { get; set; }
}