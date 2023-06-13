using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloExport
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("desc")] public string Desc { get; set; }

    [JsonProperty("descData")] public object DescData { get; set; }

    [JsonProperty("closed")] public bool Closed { get; set; }

    [JsonProperty("idOrganization")] public string IdOrganization { get; set; }

    [JsonProperty("limits")] public TrelloWelcomeLimits Limits { get; set; }

    [JsonProperty("pinned")] public bool Pinned { get; set; }

    [JsonProperty("starred")] public bool Starred { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }

    [JsonProperty("prefs")] public TrelloWelcomePrefs Prefs { get; set; }

    [JsonProperty("shortLink")] public string ShortLink { get; set; }

    [JsonProperty("subscribed")] public bool Subscribed { get; set; }

    [JsonProperty("labelNames")] public TrelloLabelNames LabelNames { get; set; }

    [JsonProperty("powerUps")] public object[] PowerUps { get; set; }

    [JsonProperty("dateLastActivity")] public DateTimeOffset? DateLastActivity { get; set; }

    [JsonProperty("dateLastView")] public DateTimeOffset? DateLastView { get; set; }

    [JsonProperty("shortUrl")] public Uri ShortUrl { get; set; }

    [JsonProperty("idTags")] public object[] IdTags { get; set; }

    [JsonProperty("datePluginDisable")] public object DatePluginDisable { get; set; }

    [JsonProperty("creationMethod")] public object CreationMethod { get; set; }

    [JsonProperty("ixUpdate")] public long IxUpdate { get; set; }

    [JsonProperty("templateGallery")] public object TemplateGallery { get; set; }

    [JsonProperty("actions")] public TrelloAction[] Actions { get; set; }

    [JsonProperty("cards")] public TrelloCardElement[] Cards { get; set; }

    [JsonProperty("labels")] public TrelloLabel[] Labels { get; set; }

    [JsonProperty("lists")] public TrelloListElement[] Lists { get; set; }

    [JsonProperty("members")] public TrelloMemberElement[] Members { get; set; }

    [JsonProperty("checklists")] public TrelloChecklist[] Checklists { get; set; }

    [JsonProperty("customFields")] public object[] CustomFields { get; set; }

    [JsonProperty("memberships")] public TrelloMembership[] Memberships { get; set; }

    [JsonProperty("pluginData")] public object[] PluginData { get; set; }
}