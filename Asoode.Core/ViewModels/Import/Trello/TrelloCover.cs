using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloCover
{
    [JsonProperty("idAttachment")] public object IdAttachment { get; set; }

    [JsonProperty("color")] public object Color { get; set; }

    [JsonProperty("idUploadedBackground")] public object IdUploadedBackground { get; set; }

    [JsonProperty("size")] public string Size { get; set; }

    [JsonProperty("brightness")] public string Brightness { get; set; }
}