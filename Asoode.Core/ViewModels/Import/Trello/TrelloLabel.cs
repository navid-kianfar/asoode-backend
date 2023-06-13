using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloLabel
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("idBoard")] public string IdBoard { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("color")] public string Color { get; set; }
}