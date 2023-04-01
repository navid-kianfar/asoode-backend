using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloCustomFieldOptions
{
    [JsonProperty("perField")] public TrelloPerBoard PerField { get; set; }
}