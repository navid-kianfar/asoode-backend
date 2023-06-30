using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record ProjectEditDto : SimpleDto
{
    public ProjectTemplate Template { get; set; }
}