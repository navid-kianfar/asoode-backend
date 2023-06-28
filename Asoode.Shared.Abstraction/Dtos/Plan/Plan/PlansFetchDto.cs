namespace Asoode.Shared.Abstraction.Dtos.Plan.Plan;

public record PlansFetchDto
{
    public UserPlanInfoDto Mine { get; set; }
    public PlanDto[] Plans { get; set; }
    public int ValueAdded { get; set; }
}