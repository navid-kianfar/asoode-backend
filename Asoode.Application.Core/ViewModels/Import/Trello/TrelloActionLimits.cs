using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloActionLimits
{
    [JsonProperty("reactions", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloReactions Reactions { get; set; }
}