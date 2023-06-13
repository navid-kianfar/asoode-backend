using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class Team
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("deletable")] public bool Deletable { get; set; }

    [JsonProperty("user_ids")] public List<string> UserIds { get; set; }

    [JsonProperty("permission_ids")] public string[] PermissionIds { get; set; }
}