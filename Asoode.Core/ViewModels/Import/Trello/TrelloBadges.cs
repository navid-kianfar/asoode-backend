using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloBadges
{
    [JsonProperty("attachmentsByType")] public TrelloAttachmentsByType AttachmentsByType { get; set; }

    [JsonProperty("location")] public bool Location { get; set; }

    [JsonProperty("votes")] public long Votes { get; set; }

    [JsonProperty("viewingMemberVoted")] public bool ViewingMemberVoted { get; set; }

    [JsonProperty("subscribed")] public bool Subscribed { get; set; }

    [JsonProperty("dueComplete")] public bool DueComplete { get; set; }

    [JsonProperty("due")] public DateTimeOffset? Due { get; set; }

    [JsonProperty("description")] public bool Description { get; set; }

    [JsonProperty("attachments")] public long Attachments { get; set; }

    [JsonProperty("comments")] public long Comments { get; set; }

    [JsonProperty("checkItemsChecked")] public long CheckItemsChecked { get; set; }

    [JsonProperty("checkItems")] public long CheckItems { get; set; }

    [JsonProperty("fogbugz")] public string Fogbugz { get; set; }
}