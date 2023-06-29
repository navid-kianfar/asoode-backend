namespace Asoode.Shared.Abstraction.Contracts;

public interface ITranslateService
{
    string[] AllowedCultures { get; set; }
    string DefaultCulture { get; set; }
    Dictionary<string, Dictionary<string, string>> Vocabulary { get; set; }

    string Get(string key, string? culture = null);
    Task LoadFromDirectory();
}