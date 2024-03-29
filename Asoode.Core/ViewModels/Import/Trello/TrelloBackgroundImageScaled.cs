using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloBackgroundImageScaled
{
    [JsonProperty("width")] public long Width { get; set; }

    [JsonProperty("height")] public long Height { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }
}