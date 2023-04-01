using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Collaboration;

public class ParsedInviteViewModel
{
    public bool OverCapacity { get; set; }
    public string[] EmailIdentities { get; set; }
    public List<InviteViewModel> InviteById { get; set; }
    public List<InviteViewModel> InviteByEmail { get; set; }
    public Guid[] IdIdentities { get; set; }
    public MemberInfoViewModel[] AllInvited { get; set; }
    public string[] NewMembers { get; set; }
}