using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.Logging;

public record PushNotificationDto : BaseDto
{
    public bool Android { get; set; }
    public string Auth { get; set; }
    public string Browser { get; set; }
    public bool Desktop { get; set; }
    public string Device { get; set; }
    public bool Enabled { get; set; }
    public string Endpoint { get; set; }
    public DateTime? ExpirationTime { get; set; }
    public bool Ios { get; set; }
    public bool Mobile { get; set; }
    public string P256dh { get; set; }
    public string Platform { get; set; }
    public bool Safari { get; set; }
    public bool Tablet { get; set; }
    public Guid UserId { get; set; }
}