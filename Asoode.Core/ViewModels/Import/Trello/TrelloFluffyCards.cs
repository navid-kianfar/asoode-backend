using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloFluffyCards
{
    [JsonProperty("openPerList")] public TrelloPerBoard OpenPerList { get; set; }

    [JsonProperty("totalPerList")] public TrelloPerBoard TotalPerList { get; set; }
}