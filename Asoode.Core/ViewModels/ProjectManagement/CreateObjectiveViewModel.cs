using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class CreateObjectiveViewModel
{
    [Required] public string Title { get; set; }
    public string Description { get; set; }
    public WorkPackageObjectiveType Type { get; set; }
    public Guid? ParentId { get; set; }
}