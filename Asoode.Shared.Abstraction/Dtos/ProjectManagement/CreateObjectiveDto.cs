using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CreateObjectiveDto
{
    [Required] public string Title { get; set; }
    public string Description { get; set; }
    public WorkPackageObjectiveType Type { get; set; }
    public Guid? ParentId { get; set; }
}