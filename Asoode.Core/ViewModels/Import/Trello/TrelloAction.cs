using System;
using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloAction
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("idMemberCreator")] public string IdMemberCreator { get; set; }

    [JsonProperty("data")] public TrelloData Data { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("date")] public DateTime Date { get; set; }

    [JsonProperty("limits")] public TrelloActionLimits Limits { get; set; }

    [JsonProperty("memberCreator")] public TrelloMemberCreatorClass MemberCreator { get; set; }

    [JsonProperty("member", NullValueHandling = NullValueHandling.Ignore)]
    public TrelloMemberCreatorClass Member { get; set; }
}