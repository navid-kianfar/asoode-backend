using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record MoveTaskDto : RepositionDto
{
    [Required] public Guid ListId { get; set; }
}