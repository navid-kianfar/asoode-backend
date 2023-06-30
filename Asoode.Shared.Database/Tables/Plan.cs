using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Plan : BaseEntity
{
    public PlanDto ToDto()
    {
        return new PlanDto
        {
            Days = Days,
            Description = Description,
            Enabled = Enabled,
            Id = Id,
            Order = Order,
            Picture = Picture,
            Title = Title,
            Type = Type,
            Unit = Unit,
            Users = Users,
            AttachmentSize = AttachmentSize,
            CanExtend = CanExtend,
            ComplexGroup = ComplexGroup,
            Project = Project,
            CreatedAt = CreatedAt,
            DiskSpace = DiskSpace,
            OneTime = OneTime,
            PlanCost = PlanCost,
            SimpleGroup = SimpleGroup,
            WorkPackage = WorkPackage,
            UpdatedAt = UpdatedAt,
            AdditionalWorkPackageCost = AdditionalWorkPackageCost,
            AdditionalSimpleGroupCost = AdditionalSimpleGroupCost,
            AdditionalComplexGroupCost = AdditionalComplexGroupCost,
            AdditionalProjectCost = AdditionalProjectCost,
            AdditionalSpaceCost = AdditionalSpaceCost,
            AdditionalUserCost = AdditionalUserCost,
            FeatureCustomField = FeatureCustomField,
            FeatureTimeSpent = FeatureTimeSpent,
            FeatureTimeValue = FeatureTimeValue,
            FeatureTimeOff = FeatureTimeOff,
            FeatureShift = FeatureShift,
            FeatureReports = FeatureReports,
            FeaturePayments = FeaturePayments,
            FeatureChat = FeatureChat,
            FeatureFiles = FeatureFiles,
            FeatureWbs = FeatureWbs,
            FeatureRoadMap = FeatureRoadMap,
            FeatureTree = FeatureTree,
            FeatureObjectives = FeatureObjectives,
            FeatureSeasons = FeatureSeasons,
            FeatureVote = FeatureVote,
            FeatureSubTask = FeatureSubTask,
            FeatureCalendar = FeatureCalendar,
            FeatureKartabl = FeatureKartabl,
            FeatureBlocking = FeatureBlocking,
            FeatureRelated = FeatureRelated,
            FeatureComplexGroup = FeatureComplexGroup,
            FeatureGroupTimeSpent = FeatureGroupTimeSpent
            // AdditionalProject = AdditionalProject,
            // Index = Index,
            // AdditionalSpace = AdditionalSpace,
            // AdditionalUser = AdditionalUser,
            // AdditionalComplexGroup = AdditionalComplexGroup,
            // AdditionalSimpleGroup = AdditionalSimpleGroup,
            // AdditionalWorkPackage = AdditionalWorkPackage,
        };
    }

    #region Props

    [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    [MaxLength(500)] public string Picture { get; set; }
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

    #endregion
}