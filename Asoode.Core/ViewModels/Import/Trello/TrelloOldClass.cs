using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloOldClass
{
    [JsonProperty("pos", NullValueHandling = NullValueHandling.Ignore)]
    public double? Pos { get; set; }

    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("idShort", NullValueHandling = NullValueHandling.Ignore)]
    public long? IdShort { get; set; }

    [JsonProperty("shortLink", NullValueHandling = NullValueHandling.Ignore)]
    public string ShortLink { get; set; }

    [JsonProperty("idMembers", NullValueHandling = NullValueHandling.Ignore)]
    public string[] IdMembers { get; set; }

    [JsonProperty("due")] public DateTimeOffset? Due { get; set; }

    [JsonProperty("idList", NullValueHandling = NullValueHandling.Ignore)]
    public string IdList { get; set; }

    [JsonProperty("idAttachmentCover", NullValueHandling = NullValueHandling.Ignore)]
    public string IdAttachmentCover { get; set; }

    [JsonProperty("closed", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Closed { get; set; }

    [JsonProperty("dueComplete", NullValueHandling = NullValueHandling.Ignore)]
    public bool? DueComplete { get; set; }

    [JsonProperty("dueReminder")] public long? DueReminder { get; set; }

    [JsonProperty("prefs", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloBoardPrefs Prefs { get; set; }
}