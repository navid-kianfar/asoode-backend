using System.ComponentModel.DataAnnotations;
using Asoode.Core.ViewModels.Collaboration;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class ImportViewModel
{
    [Required] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    public WorkPackageViewModel[] Packages { get; set; }
    public InviteViewModel[] Members { get; set; }
    public GroupViewModel[] Teams { get; set; }
    public double TotalAttachmentSize { get; set; }
}