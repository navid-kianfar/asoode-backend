using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class Transaction : BaseEntity
    {
        public Guid OrderId { get; set; }
        public TransactionStatus Status { get; set; }
        public double Amount { get; set; }
        public DateTime? ApprovedAt { get; set; }
        
        [MaxLength(200)]public string TrackingCode { get; set; }
        [MaxLength(200)]public string ReferenceNumber { get; set; }
        [MaxLength(200)]public long? ExternalId { get; set; }
        [MaxLength(1500)]public string Detail { get; set; }
    }
}