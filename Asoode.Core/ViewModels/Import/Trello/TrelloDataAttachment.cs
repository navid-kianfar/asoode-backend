using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloDataAttachment
{
    [JsonProperty("previewUrl2x")] public Uri PreviewUrl2X { get; set; }

    [JsonProperty("previewUrl")] public Uri PreviewUrl { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("id")] public string Id { get; set; }
}