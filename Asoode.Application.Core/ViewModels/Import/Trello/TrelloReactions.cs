using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloReactions
{
    [JsonProperty("perAction")] public TrelloPerBoard PerAction { get; set; }

    [JsonProperty("uniquePerAction")] public TrelloPerBoard UniquePerAction { get; set; }
}