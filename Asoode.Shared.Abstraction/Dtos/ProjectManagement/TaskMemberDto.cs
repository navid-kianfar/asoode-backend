using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record TaskMemberDto
{
    public bool IsGroup { get; set; }
    [Required] public Guid RecordId { get; set; }
}