namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record AdvancedPlayerDto
{
    public AdvancedPlayerCommentDto[] Comments { get; set; }
    public AdvancedPlayerShapeDto[] Shapes { get; set; }
}