using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloBoardPrefs
{
    [JsonProperty("background", NullValueHandling = NullValueHandling.Ignore)]
    public string Background { get; set; }

    [JsonProperty("selfJoin", NullValueHandling = NullValueHandling.Ignore)]
    public bool? SelfJoin { get; set; }

    [JsonProperty("cardCovers", NullValueHandling = NullValueHandling.Ignore)]
    public bool? CardCovers { get; set; }
}