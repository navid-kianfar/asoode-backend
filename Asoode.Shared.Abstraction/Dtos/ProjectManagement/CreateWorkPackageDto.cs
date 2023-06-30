using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record CreateWorkPackageDto : AccessDto
{
    [Required] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    public BoardTemplate? BoardTemplate { get; set; }
    public Guid? ParentId { get; set; }
}