using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloDataList
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("pos", NullValueHandling = NullValueHandling.Ignore)]
    public double? Pos { get; set; }

    [JsonProperty("closed", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Closed { get; set; }
}