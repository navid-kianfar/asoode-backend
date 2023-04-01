using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloFluffyCards
{
    [JsonProperty("openPerList")] public TrelloPerBoard OpenPerList { get; set; }

    [JsonProperty("totalPerList")] public TrelloPerBoard TotalPerList { get; set; }
}