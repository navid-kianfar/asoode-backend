using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloCustomFields
{
    [JsonProperty("perBoard")] public TrelloPerBoard PerBoard { get; set; }
}