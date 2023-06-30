namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record TreeDto
{
    public Dictionary<Guid, TreeReportDto> Tree { get; set; }
}