using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloCardLimits
{
    [JsonProperty("attachments")] public TrelloStickers Attachments { get; set; }

    [JsonProperty("checklists")] public TrelloStickers Checklists { get; set; }

    [JsonProperty("stickers")] public TrelloStickers Stickers { get; set; }
}