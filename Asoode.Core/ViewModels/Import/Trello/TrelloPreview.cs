using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloPreview
{
    [JsonProperty("_id")] public string Id { get; set; }

    [JsonProperty("id")] public string PreviewId { get; set; }

    [JsonProperty("scaled")] public bool Scaled { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }

    [JsonProperty("bytes")] public long Bytes { get; set; }

    [JsonProperty("height")] public long Height { get; set; }

    [JsonProperty("width")] public long Width { get; set; }
}