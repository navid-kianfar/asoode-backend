using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloAttachments
{
    [JsonProperty("perBoard")] public TrelloPerBoard PerBoard { get; set; }

    [JsonProperty("perCard")] public TrelloPerBoard PerCard { get; set; }
}