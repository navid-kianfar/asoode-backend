using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Transaction;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Transaction : BaseEntity
{
    public Guid OrderId { get; set; }
    public TransactionStatus Status { get; set; }
    public double Amount { get; set; }
    public DateTime? ApprovedAt { get; set; }

    [MaxLength(200)] public string TrackingCode { get; set; }
    [MaxLength(200)] public string ReferenceNumber { get; set; }
    [MaxLength(200)] public long? ExternalId { get; set; }
    [MaxLength(1500)] public string Detail { get; set; }

    public TransactionDto ToDto()
    {
        return new TransactionDto
        {
            Amount = Amount,
            ApprovedAt = ApprovedAt,
            ExternalId = ExternalId,
            Detail = Detail,
            ReferenceNumber = ReferenceNumber,
            TrackingCode = TrackingCode,
            OrderId = OrderId,
            Status = Status,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            // Index = Index,
            // FullName = FullName,
            // PlanName = PlanName,
        };
    }
}