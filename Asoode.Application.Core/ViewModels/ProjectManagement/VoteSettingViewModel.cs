using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class VoteSettingViewModel
{
    public bool? Paused { get; set; }
    public bool? Private { get; set; }
    public WorkPackageTaskVoteNecessity? Necessity { get; set; }
}