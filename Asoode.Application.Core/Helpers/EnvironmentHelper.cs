namespace Asoode.Application.Core.Helpers;

public static class EnvironmentHelper
{
    public static string CurrentDirectory => Environment.CurrentDirectory ?? "";

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

    public static string GetAppSettingPath(string fileName)
    {
        var cleaned = CurrentDirectory.Replace("\\", "/");
        return $"{cleaned.Split("/bin/")[0]}/Properties/{fileName}";
    }
}