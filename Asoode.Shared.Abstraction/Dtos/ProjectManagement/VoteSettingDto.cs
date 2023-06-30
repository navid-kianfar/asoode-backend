using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record VoteSettingDto
{
    public bool? Paused { get; set; }
    public bool? Private { get; set; }
    public WorkPackageTaskVoteNecessity? Necessity { get; set; }
}