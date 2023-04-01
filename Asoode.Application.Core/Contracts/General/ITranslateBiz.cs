namespace Asoode.Application.Core.Contracts.General;

public interface ITranslateBiz
{
    string[] AllowedCultures { get; set; }
    string DefaultCulture { get; set; }
    Dictionary<string, Dictionary<string, string>> Vocabulary { get; set; }

    string Get(string key, string culture = null);
}