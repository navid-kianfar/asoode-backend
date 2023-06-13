using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class VoteSettingViewModel
{
    public bool? Paused { get; set; }
    public bool? Private { get; set; }
    public WorkPackageTaskVoteNecessity? Necessity { get; set; }
}