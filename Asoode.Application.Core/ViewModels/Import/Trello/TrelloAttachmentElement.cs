using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloAttachmentElement
{
    [JsonProperty("bytes")] public long? Bytes { get; set; }

    [JsonProperty("date")] public DateTime Date { get; set; }

    [JsonProperty("edgeColor")] public string EdgeColor { get; set; }

    [JsonProperty("idMember")] public string IdMember { get; set; }

    [JsonProperty("isUpload")] public bool IsUpload { get; set; }

    [JsonProperty("mimeType")] public object MimeType { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("previews")] public TrelloPreview[] Previews { get; set; }

    [JsonProperty("url")] public string Url { get; set; }

    [JsonProperty("pos")] public long Pos { get; set; }

    [JsonProperty("id")] public string Id { get; set; }
}