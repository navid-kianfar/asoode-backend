using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloListLimits
{
    [JsonProperty("cards")] public TrelloFluffyCards Cards { get; set; }
}