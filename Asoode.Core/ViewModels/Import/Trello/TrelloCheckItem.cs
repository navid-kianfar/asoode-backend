using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloCheckItem
{
    [JsonProperty("idChecklist")] public string IdChecklist { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }
}