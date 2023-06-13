using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class Task
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("creator_user_id")] public string CreatorUserId { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("start_time")] public DateTime? StartTime { get; set; }

    [JsonProperty("deadline")] public DateTime? Deadline { get; set; }

    [JsonProperty("color_label")] public string ColorLabel { get; set; }

    [JsonProperty("section")] public string Section { get; set; }

    [JsonProperty("assigned_user_ids")] public string[] AssignedUserIds { get; set; }

    [JsonProperty("tags")] public string[] Tags { get; set; }

    [JsonProperty("weight")] public long? Weight { get; set; }

    [JsonProperty("cover_attachment_id")] public string CoverAttachmentId { get; set; }

    [JsonProperty("archived")] public bool Archived { get; set; }

    [JsonProperty("check_lists")] public TaskCheckList[] CheckLists { get; set; }

    [JsonProperty("attachments")] public Attachment[] Attachments { get; set; }

    [JsonProperty("comments")] public Comment[] Comments { get; set; }
}