namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record TaskLabelDto
{
    public string Title { get; set; }
    public string Color { get; set; }
    public bool Dark { get; set; }
}