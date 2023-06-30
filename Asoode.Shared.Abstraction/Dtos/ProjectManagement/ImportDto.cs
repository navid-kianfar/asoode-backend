using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Collaboration;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record ImportDto
{
    [Required] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    public WorkPackageDto[] Packages { get; set; }
    public InviteDto[] Members { get; set; }
    public GroupDto[] Teams { get; set; }
    public double TotalAttachmentSize { get; set; }
}