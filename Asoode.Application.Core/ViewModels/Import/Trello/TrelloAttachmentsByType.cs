using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloAttachmentsByType
{
    [JsonProperty("trello")] public Trello Trello { get; set; }
}