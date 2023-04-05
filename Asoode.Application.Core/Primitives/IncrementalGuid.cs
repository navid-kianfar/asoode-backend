
namespace Asoode.Application.Core.Primitives;

public class IncrementalGuid
{
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

    public static Guid NewId()
    {
        var now = DateTime.UtcNow;
        var counter = Interlocked.Increment(ref Counter);
        var endSection = long.Parse($"{counter}{now.ToString("fff")}").ToString("x").PadLeft(12, '0');
        var strId =
            $"{now.ToString("yyyyMMdd")}-{now.ToString("HHmm")}-{GetSecondPart(now.Second)}-0000-{endSection}";

        return Guid.Parse(strId);
    }
}