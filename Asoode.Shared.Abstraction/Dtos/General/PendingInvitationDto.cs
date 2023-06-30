using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record PendingInvitationDto : BaseDto
{
    public string Identifier { get; set; }
    public Guid RecordId { get; set; }
    public AccessType Access { get; set; }
}