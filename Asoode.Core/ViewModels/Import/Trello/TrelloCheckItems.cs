using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloCheckItems
{
    [JsonProperty("perChecklist")] public TrelloPerBoard PerChecklist { get; set; }
}