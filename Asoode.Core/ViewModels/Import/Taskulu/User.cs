using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class User
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("username")] public string Username { get; set; }

    [JsonProperty("name")] public string Name { get; set; }
}