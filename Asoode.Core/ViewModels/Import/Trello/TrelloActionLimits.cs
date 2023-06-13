using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloActionLimits
{
    [JsonProperty("reactions", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloReactions Reactions { get; set; }
}