using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloLabelNames
{
    [JsonProperty("green")] public string Green { get; set; }

    [JsonProperty("yellow")] public string Yellow { get; set; }

    [JsonProperty("orange")] public string Orange { get; set; }

    [JsonProperty("red")] public string Red { get; set; }

    [JsonProperty("purple")] public string Purple { get; set; }

    [JsonProperty("blue")] public string Blue { get; set; }

    [JsonProperty("sky")] public string Sky { get; set; }

    [JsonProperty("lime")] public string Lime { get; set; }

    [JsonProperty("pink")] public string Pink { get; set; }

    [JsonProperty("black")] public string Black { get; set; }
}