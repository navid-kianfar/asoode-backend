using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloStickers
{
    [JsonProperty("perCard")] public TrelloPerBoard PerCard { get; set; }
}