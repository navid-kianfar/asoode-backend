using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloCoordinate
{
    [JsonProperty("latitude")] public string Latitude { get; set; }

    [JsonProperty("longitude")] public string Longitude { get; set; }

    public override string ToString()
    {
        return $"{Latitude},{Longitude}";
    }
}