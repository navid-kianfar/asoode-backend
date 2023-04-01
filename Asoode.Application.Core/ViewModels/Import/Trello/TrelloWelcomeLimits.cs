using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloWelcomeLimits
{
    [JsonProperty("attachments")] public TrelloAttachments Attachments { get; set; }

    [JsonProperty("boards")] public TrelloBoards Boards { get; set; }

    [JsonProperty("cards")] public TrelloPurpleCards Cards { get; set; }

    [JsonProperty("checklists")] public TrelloAttachments Checklists { get; set; }

    [JsonProperty("checkItems")] public TrelloCheckItems CheckItems { get; set; }

    [JsonProperty("customFields")] public TrelloCustomFields CustomFields { get; set; }

    [JsonProperty("customFieldOptions")] public TrelloCustomFieldOptions CustomFieldOptions { get; set; }

    [JsonProperty("labels")] public TrelloCustomFields Labels { get; set; }

    [JsonProperty("lists")] public TrelloLists Lists { get; set; }

    [JsonProperty("stickers")] public TrelloStickers Stickers { get; set; }

    [JsonProperty("reactions")] public TrelloReactions Reactions { get; set; }
}