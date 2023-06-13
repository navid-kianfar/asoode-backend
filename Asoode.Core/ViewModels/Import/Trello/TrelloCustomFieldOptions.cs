using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloCustomFieldOptions
{
    [JsonProperty("perField")] public TrelloPerBoard PerField { get; set; }
}