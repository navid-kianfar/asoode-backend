using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.Collaboration;

public class GroupCreateViewModel
{
    public GroupCreateViewModel()
    {
        Members = new InviteViewModel[0];
    }

    public Guid? ParentId { get; set; }
    public InviteViewModel[] Members { get; set; }
    [Required] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    public bool Complex { get; set; }
    public GroupType Type { get; set; }
}