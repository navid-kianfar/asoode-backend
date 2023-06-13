using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class Sheet
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("deletable")] public bool Deletable { get; set; }

    [JsonProperty("lists")] public List[] Lists { get; set; }
}