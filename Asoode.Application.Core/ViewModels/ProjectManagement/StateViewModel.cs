using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class StateViewModel
{
    [Required] public WorkPackageTaskState State { get; set; }
}