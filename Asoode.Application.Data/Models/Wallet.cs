using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class Wallet : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid? OrderId { get; set; }
        public double Amount { get; set; }
        public WalletType Type { get; set; }
        public CostUnit Unit { get; set; }
    }
}