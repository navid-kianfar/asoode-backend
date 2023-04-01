using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloData
{
    [JsonProperty("old", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloOldClass Old { get; set; }

    [JsonProperty("card", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloOldClass Card { get; set; }

    [JsonProperty("board")] public TrelloBoard Board { get; set; }

    [JsonProperty("list", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloDataList List { get; set; }

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    [JsonProperty("member", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloChecklistAlt Member { get; set; }

    [JsonProperty("idMember", NullValueHandling = NullValueHandling.Ignore)]
    public string IdMember { get; set; }

    [JsonProperty("listAfter", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloChecklistAlt ListAfter { get; set; }

    [JsonProperty("listBefore", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloChecklistAlt ListBefore { get; set; }

    [JsonProperty("attachment", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloDataAttachment Attachment { get; set; }

    [JsonProperty("idMemberAdded", NullValueHandling = NullValueHandling.Ignore)]
    public string IdMemberAdded { get; set; }

    [JsonProperty("memberType", NullValueHandling = NullValueHandling.Ignore)]
    public string MemberType { get; set; }

    [JsonProperty("checklist", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloChecklistAlt Checklist { get; set; }

    [JsonProperty("checkItem", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloCheckItemAlt CheckItem { get; set; }
}