using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloPurpleCards
{
    [JsonProperty("openPerBoard")] public TrelloPerBoard OpenPerBoard { get; set; }

    [JsonProperty("openPerList")] public TrelloPerBoard OpenPerList { get; set; }

    [JsonProperty("totalPerBoard")] public TrelloPerBoard TotalPerBoard { get; set; }

    [JsonProperty("totalPerList")] public TrelloPerBoard TotalPerList { get; set; }
}