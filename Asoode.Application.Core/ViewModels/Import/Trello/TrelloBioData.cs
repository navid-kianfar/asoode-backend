using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloBioData
{
    [JsonProperty("emoji")] public TrelloNonPublic Emoji { get; set; }
}