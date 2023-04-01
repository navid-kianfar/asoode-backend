using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class ProjectMemberViewModel : BaseViewModel
{
    public MemberInfoViewModel Member { get; set; }
    public bool IsGroup { get; set; }
    public Guid RecordId { get; set; }
    public Guid ProjectId { get; set; }
    public AccessType Access { get; set; }
}