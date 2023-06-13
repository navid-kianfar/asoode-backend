using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloBioData
{
    [JsonProperty("emoji")] public TrelloNonPublic Emoji { get; set; }
}