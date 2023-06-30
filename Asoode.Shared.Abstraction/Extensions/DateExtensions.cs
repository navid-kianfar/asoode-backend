namespace Asoode.Shared.Abstraction.Extensions;

public static class DateExtensions
{
    public static string GetTime(this DateTime date)
    {
        return date.ToString("yyMMddhhmmssfff");
    }
}