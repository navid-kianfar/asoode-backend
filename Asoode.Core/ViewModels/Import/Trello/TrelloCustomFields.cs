using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloCustomFields
{
    [JsonProperty("perBoard")] public TrelloPerBoard PerBoard { get; set; }
}