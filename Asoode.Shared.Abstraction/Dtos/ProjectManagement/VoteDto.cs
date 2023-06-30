using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record VoteDto
{
    [Required] public bool Vote { get; set; }
}