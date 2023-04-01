using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloLists
{
    [JsonProperty("openPerBoard")] public TrelloPerBoard OpenPerBoard { get; set; }

    [JsonProperty("totalPerBoard")] public TrelloPerBoard TotalPerBoard { get; set; }
}