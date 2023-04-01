using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Membership.Plan;

public class PlanViewModel : BaseViewModel
{
    #region Props

    [Required] public string Title { get; set; }
    [Required] public string Description { get; set; }
    public string Picture { get; set; }
    public PlanType Type { get; set; }
    public CostUnit Unit { get; set; }
    public bool Enabled { get; set; }
    public bool OneTime { get; set; }
    public int Order { get; set; }
    public int Days { get; set; }
    public int AttachmentSize { get; set; }
    public int PlanCost { get; set; }
    public bool CanExtend { get; set; }

    #endregion

    #region Def

    public long DiskSpace { get; set; }
    public int Users { get; set; }
    public int WorkPackage { get; set; }
    public int Project { get; set; }
    public int SimpleGroup { get; set; }
    public int ComplexGroup { get; set; }

    #endregion

    #region Additional

    public int AdditionalUser { get; set; }
    public int AdditionalSpace { get; set; }
    public int AdditionalProject { get; set; }
    public int AdditionalSimpleGroup { get; set; }
    public int AdditionalComplexGroup { get; set; }
    public int AdditionalWorkPackage { get; set; }

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
    public int Index { get; set; }

    #endregion
}