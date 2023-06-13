using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class Trello
{
    [JsonProperty("board")] public long Board { get; set; }

    [JsonProperty("card")] public long Card { get; set; }
}