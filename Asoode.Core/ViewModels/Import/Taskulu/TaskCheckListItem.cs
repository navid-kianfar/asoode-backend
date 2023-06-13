using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class TaskCheckListItem
{
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }
    [JsonProperty("completed_at")] public DateTime? CompletedAt { get; set; }
}