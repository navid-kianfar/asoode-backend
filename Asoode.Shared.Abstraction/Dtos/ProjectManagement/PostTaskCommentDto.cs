using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record PostTaskCommentDto
{
    [Required] public string Message { get; set; }
    public bool Private { get; set; }
}