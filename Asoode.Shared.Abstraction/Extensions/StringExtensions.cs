namespace Asoode.Shared.Abstraction.Extensions;

public static class StringExtensions
{
    public static Tuple<string, string> SplitFullName(this string fullName)
    {
        var parts = fullName.Split(' ').ToList();
        var firstName = parts.First();
        parts.RemoveAt(0);
        var lastName = String.Join(' ', parts);
        return new Tuple<string, string>(firstName, lastName);
    }
}