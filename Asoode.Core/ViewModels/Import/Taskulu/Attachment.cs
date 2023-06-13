using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class Attachment
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("creator_user_id")] public string CreatorUserId { get; set; }

    [JsonProperty("file")] public File File { get; set; }
}