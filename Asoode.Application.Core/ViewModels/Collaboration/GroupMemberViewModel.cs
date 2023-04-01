using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Collaboration;

public class GroupMemberViewModel : BaseViewModel
{
    public MemberInfoViewModel Member { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public AccessType Access { get; set; }
}