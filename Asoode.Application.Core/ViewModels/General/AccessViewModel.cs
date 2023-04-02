using Asoode.Application.Core.ViewModels.Collaboration;

namespace Asoode.Application.Core.ViewModels.General;

public class AccessViewModel
{
    public AccessViewModel()
    {
        Groups = Array.Empty<InviteViewModel>();
        Members = Array.Empty<InviteViewModel>();
    }

    public InviteViewModel[] Groups { get; set; }
    public InviteViewModel[] Members { get; set; }
}