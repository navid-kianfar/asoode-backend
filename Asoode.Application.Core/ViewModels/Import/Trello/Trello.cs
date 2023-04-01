using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class Trello
{
    [JsonProperty("board")] public long Board { get; set; }

    [JsonProperty("card")] public long Card { get; set; }
}