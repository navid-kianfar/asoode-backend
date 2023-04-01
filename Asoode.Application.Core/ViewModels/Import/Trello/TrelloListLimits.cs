using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloListLimits
{
    [JsonProperty("cards")] public TrelloFluffyCards Cards { get; set; }
}