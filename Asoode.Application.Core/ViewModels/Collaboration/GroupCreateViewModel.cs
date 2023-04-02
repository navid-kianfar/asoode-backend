using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.Collaboration;

public class GroupCreateViewModel
{
    public GroupCreateViewModel()
    {
        Members = Array.Empty<InviteViewModel>();
    }

    public Guid? ParentId { get; set; }
    public InviteViewModel[] Members { get; set; }
    [Required] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    public bool Complex { get; set; }
    public GroupType Type { get; set; }
}