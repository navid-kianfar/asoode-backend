using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Collaboration;

public record TimeOffDto : BaseDto
{
    public DateTime BeginAt { get; set; }
    public string Description { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsHourly { get; set; }
    public Guid ResponderId { get; set; }
    public RequestStatus Status { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public int Index { get; set; }
}