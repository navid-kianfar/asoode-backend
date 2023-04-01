using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.Logging;

public class NotificationViewModel
{
    public NotificationViewModel()
    {
        Users = new Guid[0];
        PushUsers = new PushNotificationViewModel[0];
    }

    public dynamic Data { get; set; }
    public PushNotificationViewModel[] PushUsers { get; set; }
    public ActivityType Type { get; set; }
    public Guid[] Users { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string Avatar { get; set; }
    public string Url { get; set; }
    public Guid UserId { get; set; }
}