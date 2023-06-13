namespace Asoode.Core.Contracts.General;

public interface IValidateBiz
{
    bool IsEmail(string email);
    bool IsForeignMobile(string mobile);
    bool IsIranianMobile(string mobile);
    bool IsMobile(string mobile);
    string PrefixMobileNumber(string mobile);
    string RemoveSpecialChars(string input);
    bool IsDigit(string input);
}