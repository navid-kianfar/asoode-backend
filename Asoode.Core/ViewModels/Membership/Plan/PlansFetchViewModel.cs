namespace Asoode.Core.ViewModels.Membership.Plan;

public class PlansFetchViewModel
{
    public UserPlanInfoViewModel Mine { get; set; }
    public PlanViewModel[] Plans { get; set; }
    public int ValueAdded { get; set; }
}