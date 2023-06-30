using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Logging;

public record NotificationDto
{
    public dynamic Data { get; set; }
    public PushNotificationDto[] PushUsers { get; set; }
    public ActivityType Type { get; set; }
    public Guid[] Users { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string Avatar { get; set; }
    public string Url { get; set; }
    public Guid UserId { get; set; }
}