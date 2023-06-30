using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Payment;

public record OrderDto : BaseDto
{
    #region Props

    public Guid? DiscountId { get; set; }
    public OrderStatus Status { get; set; }
    public Guid UserId { get; set; }
    public Guid PlanId { get; set; }
    public PlanType Type { get; set; }
    public CostUnit Unit { get; set; }
    public bool OneTime { get; set; }
    public int Days { get; set; }
    public int AttachmentSize { get; set; }
    public bool CanExtend { get; set; }

    public double PlanCost { get; set; }
    public double TotalAmount { get; set; }
    public double? AppliedDiscount { get; set; }
    public double ValueAdded { get; set; }
    public double PaymentAmount { get; set; }

    #endregion

    #region Def

    public double DiskSpace { get; set; }
    public int Users { get; set; }
    public int WorkPackage { get; set; }
    public int Project { get; set; }
    public int SimpleGroup { get; set; }
    public int ComplexGroup { get; set; }

    #endregion

    #region Additional Cost

    public int AdditionalWorkPackageCost { get; set; }
    public int AdditionalUserCost { get; set; }
    public int AdditionalSpaceCost { get; set; }
    public int AdditionalProjectCost { get; set; }
    public int AdditionalSimpleGroupCost { get; set; }
    public int AdditionalComplexGroupCost { get; set; }

    #endregion

    #region Features

    public bool FeatureCustomField { get; set; }
    public bool FeatureTimeSpent { get; set; }
    public bool FeatureTimeValue { get; set; }
    public bool FeatureTimeOff { get; set; }
    public bool FeatureShift { get; set; }
    public bool FeatureReports { get; set; }
    public bool FeaturePayments { get; set; }
    public bool FeatureChat { get; set; }
    public bool FeatureFiles { get; set; }
    public bool FeatureWbs { get; set; }
    public bool FeatureRoadMap { get; set; }
    public bool FeatureTree { get; set; }
    public bool FeatureObjectives { get; set; }
    public bool FeatureSeasons { get; set; }
    public bool FeatureVote { get; set; }
    public bool FeatureSubTask { get; set; }
    public bool FeatureKartabl { get; set; }
    public bool FeatureCalendar { get; set; }
    public bool FeatureBlocking { get; set; }
    public bool FeatureRelated { get; set; }
    public bool FeatureComplexGroup { get; set; }
    public bool FeatureGroupTimeSpent { get; set; }
    public bool UseWallet { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool Automated { get; set; }
    public OrderType OrderType { get; set; }
    public OrderDuration Duration { get; set; }

    #endregion
}