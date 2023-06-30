using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CreateTaskDto : TitleDto
{
    [Required] public Guid ListId { get; set; }
    public Guid? ParentId { get; set; }
    public int? Count { get; set; }
}