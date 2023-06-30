using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.User;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record TaskMemberDto
{
    public bool IsGroup { get; set; }
    [Required] public Guid RecordId { get; set; }
}