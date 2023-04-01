using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class CreateWorkPackageViewModel : AccessViewModel
{
    [Required] public string Title { get; set; }
    [MaxLength(1000)] public string Description { get; set; }
    public BoardTemplate? BoardTemplate { get; set; }
    public Guid? ParentId { get; set; }
}