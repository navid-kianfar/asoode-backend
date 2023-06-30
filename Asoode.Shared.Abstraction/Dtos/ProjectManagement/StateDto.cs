using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record StateDto
{
    [Required] public WorkPackageTaskState State { get; set; }
}