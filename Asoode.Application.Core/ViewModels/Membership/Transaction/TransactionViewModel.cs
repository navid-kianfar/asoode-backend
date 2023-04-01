using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Membership.Transaction;

public class TransactionViewModel : BaseViewModel
{
    public Guid OrderId { get; set; }
    public TransactionStatus Status { get; set; }
    public double Amount { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public string TrackingCode { get; set; }
    public string ReferenceNumber { get; set; }
    public long? ExternalId { get; set; }
    public string Detail { get; set; }
    public int Index { get; set; }
    public string FullName { get; set; }
    public string PlanName { get; set; }
}