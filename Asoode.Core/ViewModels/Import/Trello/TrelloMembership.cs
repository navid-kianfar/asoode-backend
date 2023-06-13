using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloMembership
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("idMember")] public string IdMember { get; set; }

    [JsonProperty("memberType")] public string MemberType { get; set; }

    [JsonProperty("unconfirmed")] public bool Unconfirmed { get; set; }

    [JsonProperty("deactivated")] public bool Deactivated { get; set; }
}