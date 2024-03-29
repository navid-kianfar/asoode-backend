using Asoode.Core.ViewModels.Collaboration;

namespace Asoode.Core.ViewModels.General;

public class AccessViewModel
{
    public AccessViewModel()
    {
        Groups = new InviteViewModel[0];
        Members = new InviteViewModel[0];
    }

    public InviteViewModel[] Groups { get; set; }
    public InviteViewModel[] Members { get; set; }
}