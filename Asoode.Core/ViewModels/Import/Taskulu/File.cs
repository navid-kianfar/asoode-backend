using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class File
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("size")] public long Size { get; set; }

    [JsonProperty("mime_type")] public string MimeType { get; set; }

    [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
    public string Source { get; set; }

    [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
    public string Url { get; set; }

    [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
    public Uri Thumbnail { get; set; }
}