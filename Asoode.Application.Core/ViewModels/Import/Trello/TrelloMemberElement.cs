using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloMemberElement
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("activityBlocked")] public bool ActivityBlocked { get; set; }

    [JsonProperty("avatarHash")] public string AvatarHash { get; set; }

    [JsonProperty("avatarUrl")] public object AvatarUrl { get; set; }

    [JsonProperty("bio")] public string Bio { get; set; }

    [JsonProperty("bioData")] public TrelloBioData BioData { get; set; }

    [JsonProperty("confirmed")] public bool Confirmed { get; set; }

    [JsonProperty("fullName")] public string FullName { get; set; }

    [JsonProperty("idEnterprise")] public object IdEnterprise { get; set; }

    [JsonProperty("idEnterprisesDeactivated")]
    public object[] IdEnterprisesDeactivated { get; set; }

    [JsonProperty("idMemberReferrer")] public object IdMemberReferrer { get; set; }

    [JsonProperty("idPremOrgsAdmin")] public object[] IdPremOrgsAdmin { get; set; }

    [JsonProperty("initials")] public string Initials { get; set; }

    [JsonProperty("memberType")] public string MemberType { get; set; }

    [JsonProperty("nonPublic")] public TrelloNonPublic NonPublic { get; set; }

    [JsonProperty("nonPublicAvailable")] public bool NonPublicAvailable { get; set; }

    [JsonProperty("products")] public object[] Products { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }

    [JsonProperty("username")] public string Username { get; set; }

    [JsonProperty("status")] public string Status { get; set; }
}