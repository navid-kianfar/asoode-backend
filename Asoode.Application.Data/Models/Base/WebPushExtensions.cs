namespace Asoode.Application.Data.Models.Base
{
    public static class WebPushExtensions
    {
        public static PushNotificationViewModel ToViewModel(this WebPush push)
        {
            return new PushNotificationViewModel
            {
                Android = push.Android,
                Auth = push.Auth,
                Browser = push.Browser,
                Desktop = push.Desktop,
                Device = push.Device,
                Enabled = push.Enabled,
                Endpoint = push.Endpoint,
                ExpirationTime = push.ExpirationTime,
                Ios = push.Ios,
                Mobile = push.Mobile,
                P256dh = push.P256dh,
                Platform = push.Platform,
                Safari = push.Safari,
                Tablet = push.Tablet,
                UserId = push.UserId,
            };
        }
        public static DeviceViewModel ToDeviceViewModel(this WebPush push)
        {
            return new DeviceViewModel
            {
                Enabled = push.Enabled,
                Id = push.Id,
                Os = push.Platform,
                Title = push.Title,
                CreatedAt = push.CreatedAt,
                UpdatedAt = push.UpdatedAt
            };
        }
    }
}