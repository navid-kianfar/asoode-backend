using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record RepositionDto
{
    [Required] public int Order { get; set; }
}