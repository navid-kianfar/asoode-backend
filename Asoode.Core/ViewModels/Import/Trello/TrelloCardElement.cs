using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloCardElement
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("address")] public object Address { get; set; }

    [JsonProperty("checkItemStates")] public object CheckItemStates { get; set; }

    [JsonProperty("closed")] public bool Closed { get; set; }

    [JsonProperty("coordinates")] public TrelloCoordinate Coordinates { get; set; }

    [JsonProperty("creationMethod")] public object CreationMethod { get; set; }

    [JsonProperty("dateLastActivity")] public DateTime? DateLastActivity { get; set; }

    [JsonProperty("desc")] public string Desc { get; set; }

    [JsonProperty("descData")] public object DescData { get; set; }

    [JsonProperty("dueReminder")] public long? DueReminder { get; set; }

    [JsonProperty("idBoard")] public string IdBoard { get; set; }

    [JsonProperty("idLabels")] public string[] IdLabels { get; set; }

    [JsonProperty("idList")] public string IdList { get; set; }

    [JsonProperty("idMembersVoted")] public object[] IdMembersVoted { get; set; }

    [JsonProperty("idShort")] public long IdShort { get; set; }

    [JsonProperty("idAttachmentCover")] public string IdAttachmentCover { get; set; }

    [JsonProperty("limits")] public TrelloCardLimits Limits { get; set; }

    [JsonProperty("locationName")] public object LocationName { get; set; }

    [JsonProperty("manualCoverAttachment")]
    public bool ManualCoverAttachment { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("pos")] public double Pos { get; set; }

    [JsonProperty("shortLink")] public string ShortLink { get; set; }

    [JsonProperty("badges")] public TrelloBadges Badges { get; set; }

    [JsonProperty("dueComplete")] public bool DueComplete { get; set; }

    [JsonProperty("due")] public DateTime? Due { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("idChecklists")] public object[] IdChecklists { get; set; }

    [JsonProperty("idMembers")] public string[] IdMembers { get; set; }

    [JsonProperty("labels")] public dynamic[] Labels { get; set; }

    [JsonProperty("shortUrl")] public Uri ShortUrl { get; set; }

    [JsonProperty("subscribed")] public bool Subscribed { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }

    [JsonProperty("cover")] public TrelloCover Cover { get; set; }

    [JsonProperty("attachments")] public TrelloAttachmentElement[] Attachments { get; set; }

    [JsonProperty("pluginData")] public object[] PluginData { get; set; }

    [JsonProperty("customFieldItems")] public object[] CustomFieldItems { get; set; }
}