using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class StateViewModel
{
    [Required] public WorkPackageTaskState State { get; set; }
}