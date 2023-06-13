using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Collaboration;

public class GroupMemberViewModel : BaseViewModel
{
    public MemberInfoViewModel Member { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public AccessType Access { get; set; }
}