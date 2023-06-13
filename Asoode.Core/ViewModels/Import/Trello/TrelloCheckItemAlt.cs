using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloCheckItemAlt
{
    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("id")] public string Id { get; set; }
}