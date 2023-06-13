using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class Permission
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("object_id")] public string ObjectId { get; set; }

    [JsonProperty("object_type")] public string ObjectType { get; set; }

    [JsonProperty("ability")] public string Ability { get; set; }
}