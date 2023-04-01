using System.Text.RegularExpressions;
using Asoode.Application.Core.Contracts.General;

namespace Asoode.Application.Core.Helpers;

public class ValidateBiz : IValidateBiz
{
    private readonly Regex DigitRegex;
    private readonly Regex EmailRegex;
    private readonly string EmailRegexStr;
    private readonly Regex IranianMobileRegex;
    private readonly Regex MobileInvalidCharsRegex;

    public ValidateBiz()
    {
        DigitRegex = new Regex("^[0-9]+$");
        EmailRegexStr =
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        MobileInvalidCharsRegex = new Regex(@"(""[""]+""|[""\s-,]+)");
        IranianMobileRegex =
            new Regex(
                @"(\b|\W)(((0?<=0|0)|\+98|0098|98)?([ ]|,|-|[()]){0,2}9[0|1|2|3|4|9]([ ]|,|-|[()]){0,2}(?:[0-9]([ ]|,|-|[()]){0,2}){8})\b");
        EmailRegex = new Regex(EmailRegexStr);
    }

    public bool IsEmail(string email)
    {
        return Regex.IsMatch(email, EmailRegexStr, RegexOptions.IgnoreCase);
    }

    public bool IsForeignMobile(string mobile)
    {
        if (string.IsNullOrEmpty(mobile)) return false;
        mobile = PrefixMobileNumber(mobile);
        if (string.IsNullOrEmpty(mobile)) return false;
        return !mobile.StartsWith("0098");
    }

    public bool IsIranianMobile(string mobile)
    {
        if (string.IsNullOrEmpty(mobile)) return false;
        return IranianMobileRegex.IsMatch(mobile);
    }

    public bool IsMobile(string mobile)
    {
        return IsIranianMobile(mobile) || IsForeignMobile(mobile);
    }

    public string PrefixMobileNumber(string mobile)
    {
        mobile = MobileInvalidCharsRegex.Replace((mobile ?? "").Replace("+", "00").Trim(), "");
        if (!DigitRegex.IsMatch(mobile) || string.IsNullOrEmpty(mobile) ||
            string.IsNullOrWhiteSpace(mobile)) return null;
        if (mobile.StartsWith("0") && !mobile.StartsWith("00")) mobile = "0098" + mobile.Remove(0, 1);
        if (IranianMobileRegex.IsMatch(mobile) && mobile.Length == 10) mobile = "0098" + mobile;
        if (!mobile.StartsWith("00")) mobile = "00" + mobile;
        return mobile;
    }

    public string RemoveSpecialChars(string input)
    {
        return Regex.Replace(input, @"([^a-zA-Z0-9_]|^\s)", string.Empty);
    }

    public bool IsDigit(string input)
    {
        return DigitRegex.IsMatch(input);
    }
}