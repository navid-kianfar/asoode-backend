using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class CreateObjectiveViewModel
{
    [Required] public string Title { get; set; }
    public string Description { get; set; }
    public WorkPackageObjectiveType Type { get; set; }
    public Guid? ParentId { get; set; }
}