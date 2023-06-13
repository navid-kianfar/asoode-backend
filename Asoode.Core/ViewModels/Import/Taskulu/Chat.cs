using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class Chat
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("creator_user_id")] public string CreatorUserId { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("deletable")] public bool Deletable { get; set; }

    [JsonProperty("user_ids")] public string[] UserIds { get; set; }

    [JsonProperty("attachments")] public object[] Attachments { get; set; }

    [JsonProperty("messages")] public object[] Messages { get; set; }
}