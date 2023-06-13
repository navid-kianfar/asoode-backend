using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class Taskulu
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonProperty("creator_user_id")] public string CreatorUserId { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("colors")] public string[] Colors { get; set; }

    [JsonProperty("sections")] public string[] Sections { get; set; }

    [JsonProperty("weekly_report")] public WeeklyReport WeeklyReport { get; set; }

    [JsonProperty("weight_unit")] public string WeightUnit { get; set; }

    [JsonProperty("memberships")] public Membership[] Memberships { get; set; }

    [JsonProperty("users")] public List<User> Users { get; set; }

    [JsonProperty("sheets")] public Sheet[] Sheets { get; set; }

    [JsonProperty("chats")] public Chat[] Chats { get; set; }

    [JsonProperty("timelogs")] public object[] Timelogs { get; set; }

    [JsonProperty("teams")] public Team[] Teams { get; set; }

    [JsonProperty("permissions")] public Permission[] Permissions { get; set; }

    [JsonProperty("exported_at")] public DateTime ExportedAt { get; set; }

    [JsonProperty("export_version")] public long ExportVersion { get; set; }
}