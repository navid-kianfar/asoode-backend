using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Collaboration;

public class GroupViewModel : BaseViewModel
{
    public GroupViewModel()
    {
        Members = Array.Empty<GroupMemberViewModel>();
        Pending = Array.Empty<PendingInvitationViewModel>();
    }

    public DateTime? ArchivedAt { get; set; }
    public Guid UserId { get; set; }
    [Required] public string Title { get; set; }
    public string SubTitle { get; set; }
    public string BrandTitle { get; set; }
    public string SupervisorName { get; set; }
    public string SupervisorNumber { get; set; }
    public string ResponsibleName { get; set; }
    public string ResponsibleNumber { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }
    public string Website { get; set; }
    public string PostalCode { get; set; }
    public string Address { get; set; }
    public string Tel { get; set; }
    public string Fax { get; set; }
    public string GeoLocation { get; set; }
    public string NationalId { get; set; }
    public string RegistrationId { get; set; }
    public GroupType Type { get; set; }
    public DateTime? ExpireAt { get; set; }
    public string Avatar { get; set; }
    public Guid? ParentId { get; set; }
    public bool Premium { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public int? Offices { get; set; }
    public int? Employees { get; set; }
    public GroupMemberViewModel[] Members { get; set; }
    public Guid RootId { get; set; }
    public int Level { get; set; }
    public PendingInvitationViewModel[] Pending { get; set; }
    public int AttachmentSize { get; set; }
    public bool Complex { get; set; }
}