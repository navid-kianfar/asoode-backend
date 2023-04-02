using Asoode.Application.Core.Extensions;

namespace Asoode.Application.Core.Helpers;

public class IncrementalGuid
{
    private static string? _appId = null;
    private static long Counter;

    private static string GetSecondPart(int second)
    {
        if (second <= 10)
            return $"0a{second.ToString().PadLeft(2, '0')}";
        if (second <= 20)
            return $"0b{second}";
        if (second <= 30)
            return $"0c{second}";
        if (second <= 40)
            return $"0d{second}";
        if (second <= 50)
            return $"0e{second}";
        return $"0f{second}";
    }

    private static void ThrowExceptionIfInvalidAppId(string appId)
    {

        if (!appId.IsValidHexNumber(minLength: 4, maxLength: 4))
            throw new ArgumentOutOfRangeException(nameof(appId),
                "Value of the appId argument should be a valid 4 digits hex number.");
    }

    public static Guid NewId()
    {
        if (string.IsNullOrWhiteSpace(_appId))
        {
            string? appId = EnvironmentHelper.Get("APP_ID");
            
            if (string.IsNullOrEmpty(appId)) 
                throw new ArgumentNullException("APP_ID is not provided in environment");

            ThrowExceptionIfInvalidAppId(appId);

            _appId = appId;
        }

        return NewId(_appId);
    }

    /// <summary>
    ///     Generates an incremental guid
    /// </summary>
    /// <param name="appId">Value of the appId argument should be a valid 4 digits hex number.</param>
    /// <returns></returns>
    public static Guid NewId(string appId)
    {
        ThrowExceptionIfInvalidAppId(appId);

        var now = DateTime.UtcNow;
        var counter = Interlocked.Increment(ref Counter);
        var endSection = long.Parse($"{counter}{now.ToString("fff")}").ToString("x").PadLeft(12, '0');
        var strId =
            $"{now.ToString("yyyyMMdd")}-{now.ToString("HHmm")}-{GetSecondPart(now.Second)}-{appId}-{endSection}";

        return Guid.Parse(strId);
    }
}