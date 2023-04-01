using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloBoard
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("shortLink")] public string ShortLink { get; set; }

    [JsonProperty("prefs", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloBoardPrefs Prefs { get; set; }
}