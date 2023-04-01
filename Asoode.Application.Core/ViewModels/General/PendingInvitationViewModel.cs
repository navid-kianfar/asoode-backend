using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.General;

public class PendingInvitationViewModel : BaseViewModel
{
    public string Identifier { get; set; }
    public Guid RecordId { get; set; }
    public AccessType Access { get; set; }
}