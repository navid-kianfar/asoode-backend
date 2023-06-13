using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class List
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("archived")] public bool Archived { get; set; }

    [JsonProperty("tasks")] public Task[] Tasks { get; set; }
}