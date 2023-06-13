using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloPerBoard
{
    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("disableAt")] public long DisableAt { get; set; }

    [JsonProperty("warnAt")] public long WarnAt { get; set; }
}