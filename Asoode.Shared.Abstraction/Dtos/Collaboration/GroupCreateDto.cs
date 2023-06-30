using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record GroupCreateDto
{
    public GroupCreateDto()
    {
        Members = Array.Empty<InviteDto>();
    }

    public Guid? ParentId { get; set; }
    public InviteDto[] Members { get; set; }
    [Required] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    public bool Complex { get; set; }
    public GroupType Type { get; set; }
}