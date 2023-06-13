using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloLists
{
    [JsonProperty("openPerBoard")] public TrelloPerBoard OpenPerBoard { get; set; }

    [JsonProperty("totalPerBoard")] public TrelloPerBoard TotalPerBoard { get; set; }
}