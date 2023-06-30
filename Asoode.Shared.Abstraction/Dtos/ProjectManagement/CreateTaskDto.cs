using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CreateTaskDto : TitleDto
{
    [Required] public Guid ListId { get; set; }
    public Guid? ParentId { get; set; }
    public int? Count { get; set; }
}