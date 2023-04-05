using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Asoode.Application.Core.Helpers;

public static class AppSettingHelper
{
    public static void Configure(string fileName = "launchSettings.json")
    {
        if (string.IsNullOrWhiteSpace(EnvironmentHelper.Get("APP_ENV")))
        {
            var settingPath = EnvironmentHelper.GetAppSettingPath(fileName);
            if (File.Exists(settingPath))
            {
                var settingContent = File.ReadAllText(settingPath);
                var profiles = JsonConvert.DeserializeObject<JObject>(settingContent)!["profiles"]!;
                var profile = ((JObject) profiles).Properties().First().Name;
                var config = profiles[profile]!["environmentVariables"]!;
                foreach (var property in ((JObject) config).Properties())
                    EnvironmentHelper.Set(property.Name, config[property.Name]!.ToString());
            }
        }
    }
}