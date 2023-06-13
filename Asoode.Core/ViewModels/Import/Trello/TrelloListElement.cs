using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloListElement
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("closed")] public bool Closed { get; set; }

    [JsonProperty("idBoard")] public string IdBoard { get; set; }

    [JsonProperty("pos")] public double Pos { get; set; }

    [JsonProperty("subscribed")] public bool Subscribed { get; set; }

    [JsonProperty("softLimit")] public object SoftLimit { get; set; }

    [JsonProperty("limits")] public TrelloListLimits Limits { get; set; }

    [JsonProperty("creationMethod")] public object CreationMethod { get; set; }
}