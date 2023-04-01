using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloChecklistAlt
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("id")] public string Id { get; set; }
}