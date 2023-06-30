using System.Text.Json;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Helpers;

public static class EnvironmentHelper
{
    public static string? Get(string key)
    {
        return Environment.GetEnvironmentVariable(key);
    }

    public static void Set(string key, string value)
    {
        Environment.SetEnvironmentVariable(key, value);
    }

    public static string Env()
    {
        return Get("APP_ENV") ?? "";
    }

    public static string Culture => Get("APP_I18N") ?? "";
    public static string CurrentDirectory => Environment.CurrentDirectory;

    public static bool IsDevelopment()
    {
        return Get("APP_ENV") == "Development";
    }

    public static bool IsProduction()
    {
        return Get("APP_ENV") == "Production";
    }

    public static bool IsTest()
    {
        return Get("APP_ENV") == "Test";
    }

    public static void Configure(string fileName = "launchSettings.json")
    {
        if (string.IsNullOrWhiteSpace(Env()))
            try
            {
                var settingPath = GetAppSettingPath(fileName);
                if (!string.IsNullOrEmpty(settingPath))
                {
                    Console.WriteLine("AppSetting found...");
                    var settingContent = File.ReadAllText(settingPath);
                    var envObj = JsonSerializer.Deserialize<EnvRootDto>(settingContent)!.profiles.First().Value;
                    foreach (var env in envObj.environmentVariables) Set(env.Key, env.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }

    private static string GetAppSettingPath(string fileName)
    {
        var path = Path.Combine(CurrentDirectory, "appSettings.json");
        if (File.Exists(path)) return path;

        path = CurrentDirectory.Replace("\\", "/").Split("/bin/")[0];
        path = $"{path}/Properties/{fileName}";
        if (File.Exists(path)) return path;
        return string.Empty;
    }
}