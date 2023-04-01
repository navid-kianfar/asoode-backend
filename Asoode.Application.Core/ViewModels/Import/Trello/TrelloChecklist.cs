using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloChecklist
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("idBoard")] public string IdBoard { get; set; }

    [JsonProperty("idCard")] public string IdCard { get; set; }

    [JsonProperty("checkItems")] public List<TrelloCheckItem> CheckItems { get; set; }
}