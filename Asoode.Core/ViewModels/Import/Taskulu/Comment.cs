using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class Comment
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("creator_user_id")] public string CreatorUserId { get; set; }

    [JsonProperty("content")] public string Content { get; set; }

    [JsonProperty("attachment_id")] public string AttachmentId { get; set; }
}