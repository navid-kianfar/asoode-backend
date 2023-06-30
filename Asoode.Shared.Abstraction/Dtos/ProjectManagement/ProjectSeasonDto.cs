namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record ProjectSeasonDto : BaseDto
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}