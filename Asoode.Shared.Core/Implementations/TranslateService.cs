using System.Text;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Helpers;

namespace Asoode.Shared.Core.Implementations;

internal class TranslateService : ITranslateService
{
    private readonly IJsonService _jsonService;

    public TranslateService(IJsonService jsonService)
    {
        _jsonService = jsonService;
    }
    
    public string[] AllowedCultures { get; set; } = Array.Empty<string>();
    public string DefaultCulture { get; set; } = String.Empty;
    public Dictionary<string, Dictionary<string, string>> Vocabulary { get; set; } = new();
    public string Get(string key, string? culture = null)
    {
        culture ??= EnvironmentHelper.Culture;
        if (Vocabulary[culture].TryGetValue(key, out var value))
            return value ?? key;
        return key;
    }

    public async Task LoadFromDirectory()
    {
        DefaultCulture = EnvironmentHelper.Culture;
        AllowedCultures = new[] { EnvironmentHelper.Culture };
        
        var path = Path.Combine(EnvironmentHelper.CurrentDirectory, "i18n");
        var files = new DirectoryInfo(path).GetFiles("*.json");
        foreach (var file in files)
        {
            var culture = Path.GetFileNameWithoutExtension(file.Name);
            var content = await File.ReadAllTextAsync(file.FullName, Encoding.UTF8);
            var obj = _jsonService.Deserialize<Dictionary<string, string>>(content);
            Vocabulary.Add(culture, obj);
        }
    }
}