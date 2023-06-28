using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

internal class UserPlanInfo : BaseEntity
{
    #region Props

    public Guid UserId { get; set; }
    public Guid PlanId { get; set; }
    [MaxLength(500)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    [MaxLength(500)] public string Picture { get; set; }
    public PlanType Type { get; set; }
    public CostUnit Unit { get; set; }
    public bool Enabled { get; set; }
    public bool OneTime { get; set; }
    public int Days { get; set; }
    public int AttachmentSize { get; set; }
    public bool CanExtend { get; set; }
    public DateTime? ExpireAt { get; set; }
    public Guid? OrderId { get; set; }
    public double PlanCost { get; set; }
    [DefaultValue(OrderDuration.Monthly)] public OrderDuration Duration { get; set; }

    #endregion

    #region Def

    public double Space { get; set; }
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

    #region Used

    public int UsedUser { get; set; }
    public long UsedSpace { get; set; }
    public int UsedProject { get; set; }
    public int UsedSimpleGroup { get; set; }
    public int UsedComplexGroup { get; set; }
    public int UsedWorkPackage { get; set; }

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

    #endregion
}